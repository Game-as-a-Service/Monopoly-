using System.Net.Http.Headers;
using System.Net.Http.Json;
using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.ApplicationLayer.Application.ReadyRoomUsecases.Commands;
using Monopoly.DomainLayer.ReadyRoom;

namespace Monopoly.InterfaceAdapterLayer.Server.Tests.AcceptanceTests.ReadyRoom;

public abstract class AbstractReadyRoomTestBase
{
    private protected MonopolyTestServer Server = default!;
    private protected IRepository<ReadyRoomAggregate> ReadyRoomRepository = default!;

    [TestInitialize]
    public void Setup()
    {
        Server = new MonopolyTestServer();
        ReadyRoomRepository = Server.GetRequiredService<IRepository<ReadyRoomAggregate>>();
    }

    [TestCleanup]
    public void Cleanup()
    {
        Server.Dispose();
    }

    protected async Task<string> CreateUser(string userName)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"/dev/user?userName={userName}");
        var client = Server.CreateClient();
        var response = await client.SendAsync(request);
        var token = await response.Content.ReadFromJsonAsync<string>();
        return token!;
    }

    protected async Task<string> CreateRoom(string token)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/dev/create-room");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var client = Server.CreateClient();
        var response = await client.SendAsync(request);
        var roomId = await response.Content.ReadFromJsonAsync<string>();
        return roomId!;
    }
}