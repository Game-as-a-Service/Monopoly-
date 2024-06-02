using Microsoft.AspNetCore.SignalR;
using Monopoly.DomainLayer.ReadyRoom.Events;
using Monopoly.InterfaceAdapterLayer.Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.ReadyRoom;

namespace Monopoly.InterfaceAdapterLayer.Server.Hubs.ReadyRoom.EventHandlers;

public class PlayerSelectLocationEventHandler(IHubContext<ReadyRoomHub, IReadyRoomResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerLocationSelectedEvent>
{
    protected override Task HandleSpecificEvent(PlayerLocationSelectedEvent e)
    {
        return hubContext.Clients.All.PlayerSelectLocationEvent(new PlayerSelectLocationEventArgs(e.PlayerId, (int)e.Location));
    }
}