using Microsoft.AspNetCore.SignalR;
using Monopoly.DomainLayer.ReadyRoom.Events;
using Monopoly.InterfaceAdapterLayer.Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.ReadyRoom;

namespace Monopoly.InterfaceAdapterLayer.Server.Hubs.ReadyRoom.EventHandlers;

public sealed class PlayerJoinReadyRoomEventHandler(IHubContext<ReadyRoomHub, IReadyRoomResponses> hubContext) : MonopolyEventHandlerBase<PlayerJoinReadyRoomEvent>
{
    protected override Task HandleSpecificEvent(PlayerJoinReadyRoomEvent e)
    {
        return hubContext.Clients.All.PlayerJoinReadyRoomEvent(new PlayerJoinReadyRoomEventArgs(e.PlayerId));
    }
}