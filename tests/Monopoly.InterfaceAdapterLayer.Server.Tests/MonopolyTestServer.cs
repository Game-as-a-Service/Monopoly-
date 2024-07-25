using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Monopoly.ApplicationLayer.Application.MonopolyUsecases.Commands;
using Monopoly.InterfaceAdapterLayer.Server.Configurations;
using Monopoly.InterfaceAdapterLayer.Server.Tests.Usecases;
using Monopoly.InterfaceAdapters.Server.Tests.Generators;
using SharedLibrary;
using SharedLibrary.ResponseArgs.ReadyRoom.Models;

namespace Monopoly.InterfaceAdapterLayer.Server.Tests;

internal class MonopolyTestServer : WebApplicationFactory<Program>
{
    private HttpClient Client { get; }

    public MonopolyTestServer()
    {
        Client = CreateClient();
    }

    public T GetRequiredService<T>()
        where T : notnull
    {
        using var serviceScope = Server.Services.CreateScope();
        return serviceScope.ServiceProvider.GetRequiredService<T>();
    }

    public async Task<MonopolyAssertionHub> CreateHubConnectionAsync(string gameId, string playerId)
    {
        var uri = new UriBuilder(Client.BaseAddress!)
        {
            Path = "/monopoly",
            Query = $"gameid={gameId}"
        }.Uri;
        var hub = new HubConnectionBuilder()
            .WithUrl(uri, opt =>
            {
                opt.Transports = HttpTransportType.ServerSentEvents;
                opt.AccessTokenProvider = async () => await Task.FromResult(CreateJwtToken(playerId));
                opt.HttpMessageHandlerFactory = _ => Server.CreateHandler();
            });
        MonopolyAssertionHub monopolyAssertionHub = new(hub);
        await monopolyAssertionHub.StartAsync();

        return monopolyAssertionHub;
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
                opt.Transports = HttpTransportType.ServerSentEvents;
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

[AssertionHub(typeof(IMonopolyRequests), typeof(IMonopolyResponses))]
public partial class MonopolyAssertionHub;

public interface IMonopolyRequests
{
    Task PlayerRollDice();
    Task PlayerChooseDirection(string direction);
    Task PlayerBuyLand(string blockId);
    Task PlayerPayToll();
    Task PlayerBuildHouse();
    Task PlayerMortgage(string blockId);
    Task PlayerRedeem(string blockId);
    Task PlayerBid(decimal bidPrice);
    Task EndAuction();
    Task EndRound();
    Task Settlement();
}