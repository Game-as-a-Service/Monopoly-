using Application.Usecases;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using ServerTests.Usecases;
using SharedLibrary;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using Monopoly.InterfaceAdapters.Server.Tests.Generators;
using Server.Configurations;
using ServerTests.Common;
using SharedLibrary.ResponseArgs.ReadyRoom.Models;

namespace ServerTests;

internal class MonopolyTestServer : WebApplicationFactory<Program>
{
    public HttpClient Client { get; }

    public MonopolyTestServer()
    {
        Client = CreateClient();
    }

    public T GetRequiredService<T>()
        where T : notnull
    {
        return Server.Services.CreateScope().ServiceProvider.GetRequiredService<T>();
    }

    public async Task<VerificationHub> CreateHubConnectionAsync(string gameId, string playerId)
    {
        var uri = new UriBuilder(Client.BaseAddress!)
        {
            Path = $"/monopoly",
            Query = $"gameid={gameId}"
        }.Uri;
        var hub = new HubConnectionBuilder()
            .WithUrl(uri, opt =>
            {
                opt.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.ServerSentEvents;
                opt.AccessTokenProvider = async () => await Task.FromResult(CreateJwtToken(playerId));
                opt.HttpMessageHandlerFactory = _ => Server.CreateHandler();
            })
            .Build();
        VerificationHub verificationHub = new(hub);
        await hub.StartAsync();

        return verificationHub;
    }


    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<PlayerRollDiceUsecase>();
            services.AddScoped<PlayerRollDiceUsecase, MockPlayerRollDiceUsecase>();
            services.AddSingleton<MockDiceService>();
        });
    }

    public async Task<ReadyRoomAssertionHub> CreateReadyRoomHubConnectionAsync(string gameId, string playerId)
    {
        var uri = new UriBuilder(Client.BaseAddress!)
        {
            Path = $"/ready-room",
            Query = $"gameid={gameId}"
        }.Uri;
        var builder = new HubConnectionBuilder()
            .WithUrl(uri, opt =>
            {
                opt.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.ServerSentEvents;
                opt.AccessTokenProvider = async () => await Task.FromResult(CreateJwtToken(playerId));
                opt.HttpMessageHandlerFactory = _ => Server.CreateHandler();
            });
        ReadyRoomAssertionHub readyRoomAssertionHub = new(builder);
        await readyRoomAssertionHub.StartAsync();

        return readyRoomAssertionHub;
    }

    private string CreateJwtToken(string userId)
    {
        var jwtSettings = GetRequiredService<IOptions<JwtSettings>>();
        var tokenHandler = new JwtSecurityTokenHandler();
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Value.Internal.SecretKey));
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, userId)
            }),
            Expires = DateTime.UtcNow.AddMinutes(jwtSettings.Value.Internal.ExpiresInMinutes),
            Issuer = jwtSettings.Value.Internal.Issuer,
            Audience = jwtSettings.Value.Internal.Audience,
            SigningCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);
        return tokenString;
    }
}

[AssertionHub(typeof(IReadyRoomRequests), typeof(IReadyRoomResponses))]
public partial class ReadyRoomAssertionHub;

public interface IReadyRoomRequests
{
    Task StartGame();
    Task PlayerReady();
    Task SelectLocation(int location);
    Task SelectRole(string role);
    Task JoinRoom();
    Task<ReadyRoomInfos> GetReadyRoomInfos();
}

internal class VerificationHub
{
    private readonly HubConnection _connection;

    private readonly Dictionary<string, ConcurrentQueue<object[]>> _queues = new();

    public VerificationHub(HubConnection connection)
    {
        _connection = connection;
        ListenAllEvent();
    }

    public async void VerifyDisconnection(int delay = 1000)
    {
        await Task.Delay(delay);
        Assert.AreEqual(HubConnectionState.Disconnected, _connection.State);
    }

    // 利用反射讀出所有HubResponse的Method，並且設置On function
    private void ListenAllEvent()
    {
        var interfaceType = typeof(IMonopolyResponses);
        var methods = interfaceType.GetMethods();

        foreach (var method in methods)
        {
            var parameters = method.GetParameters();
            _queues.Add(method.Name, new());

            var parameterTypes = parameters.Select(x => x.ParameterType).ToArray();
            void handler(object?[] x) => _queues[method.Name].Enqueue(x!);
            _connection.On(method.Name, parameterTypes, handler);
        }
    }

    private void Verify(string methodName, Func<object[], bool> verify, int timeout)
    {
        try
        {
            var startTime = DateTime.Now;
            while (true)
            {
                if (_queues[methodName].TryDequeue(out var result))
                {
                    var options = new JsonSerializerOptions()
                    {
                        WriteIndented = true,
                    };

                    // 這裡應該要把 result(object[])轉成T，但目前只有一個參數
                    Assert.IsTrue(verify(result),
                        $"\n回傳結果為 {JsonSerializer.Serialize(result, options)}");
                    break;
                }

                // 如果已經斷開連線測試失敗
                if (_connection.State == HubConnectionState.Disconnected)
                {
                    if (_queues[nameof(IMonopolyResponses.PlayerJoinGameFailedEvent)].TryPeek(out var errorMessages))
                        Assert.Fail(
                            $"""
                             已經斷開連線
                             訊息:
                             {string.Join("\n", errorMessages!)}
                             """);
                }

                // 計算已經等待的時間
                var elapsedMilliseconds = (DateTime.Now - startTime).TotalMilliseconds;
                if (elapsedMilliseconds >= timeout)
                {
                    Assert.Fail(
                        $"""
                         超出預期時間 {timeout} ms，預期得到 Event【{methodName}】
                         可以嘗試檢查下面的問題:
                         1. 在 EventBus 中，缺少 Event 的傳送
                         2. 在 Usecase 中，沒有使用 EventBus.PublishAsync
                         3. 在 Domain 中，沒有 添加 Domain Event
                         """);
                }

                // 等待一段時間再繼續嘗試
                SpinWait.SpinUntil(() => false, 50);
            }
        }
        catch (KeyNotFoundException ex)
        {
            Console.WriteLine(ex.Message);
            Assert.Fail(
                $"""

                 IMonopolyResponses 中缺少 Method【{methodName}】
                 可以在 IMonopolyResponses 添加 【{methodName}】 以解決這個問題
                 """);
        }
        catch (InvalidCastException ex)
        {
            Console.WriteLine(ex.Message);
            Assert.Fail(
                $"""

                 錯誤的轉型
                 可能是【IMonopolyResponses Method的參數類型】與【驗證的參數類型】不一樣
                 """);
        }
    }

    public async Task SendAsync(string method, params object?[] args)
    {
        await _connection.SendCoreAsync(method, args);
    }

    public void Verify<T1>(string methodName, Func<T1, bool> verify, int timeout = 1000)
    {
        Verify(methodName, args => verify((T1)args[0]), timeout);
    }

    public void Verify<T1, T2>(string methodName, Func<T1, T2, bool> verify, int timeout = 1000)
    {
        Verify(methodName, args => verify((T1)args[0], (T2)args[1]), timeout);
    }

    public void Verify<T1, T2, T3>(string methodName, Func<T1, T2, T3, bool> verify, int timeout = 1000)
    {
        Verify(methodName, args => verify((T1)args[0], (T2)args[1], (T3)args[2]), timeout);
    }

    public void Verify<T1, T2, T3, T4>(string methodName, Func<T1, T2, T3, T4, bool> verify, int timeout = 1000)
    {
        Verify(methodName, args => verify((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3]), timeout);
    }

    public void Verify<T1, T2, T3, T4, T5>(string methodName, Func<T1, T2, T3, T4, T5, bool> verify, int timeout = 1000)
    {
        Verify(methodName, args => verify((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3], (T5)args[4]), timeout);
    }

    // 確認所有的Queue已經是空的了
    public void VerifyNoElseEvent()
    {
        foreach (var (method, queue) in _queues)
        {
            if (method == nameof(IMonopolyResponses.PlayerJoinGameEvent))
            {
                continue;
            }

            if (method == nameof(IMonopolyResponses.WelcomeEvent))
            {
                continue;
            }

            var options = new JsonSerializerOptions()
            {
                WriteIndented = true,
            };
            Assert.IsTrue(queue.IsEmpty,
                $"""

                 【{method}】中還有 {queue.Count} 筆資料
                 {JsonSerializer.Serialize(queue, options)}
                 """);
        }
    }
}

internal static class TestHubExtension
{
    public static IDisposable On(this HubConnection hubConnection, string methodName, Type[] parameterTypes,
        Action<object?[]> handler)
    {
        return hubConnection.On(methodName, parameterTypes, static (parameters, state) =>
        {
            var currentHandler = (Action<object?[]>)state;
            currentHandler(parameters);
            return Task.CompletedTask;
        }, handler);
    }
}