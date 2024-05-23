using Client.Pages.Ready.Entities;
using SharedLibrary;
using SharedLibrary.ResponseArgs.ReadyRoom.Models;
using SignalR.Client.Generator;

namespace Client.Pages.Ready;

[TypedHubClient(typeof(IReadyRoomRequests), typeof(IReadyRoomResponses))]
public partial class ReadyRoomHubConnection;

public interface IReadyRoomRequests
{
    Task StartGame();
    Task PlayerReady();
    Task SelectLocation(ColorEnum location);
    Task SelectRole(string role);
    Task<ReadyRoomInfos> GetReadyRoomInfos();
}
