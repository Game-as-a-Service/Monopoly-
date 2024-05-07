using Microsoft.AspNetCore.SignalR.Client;
using SharedLibrary;
using SharedLibrary.ResponseArgs.ReadyRoom;
using SignalR.Client.Generator;

namespace Client.Pages;

[TypedHubClient(typeof(MonopolyHub))]
public partial class TypedHubConnection;

public abstract class Hub<T>;

public abstract class MonopolyHub : Hub<IMonopolyResponses>
{
    public abstract Task GetReadyInfo();
    public abstract Task PlaySelectLocation(string gameId, string userId, int locationId);
}
