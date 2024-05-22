using Microsoft.AspNetCore.SignalR;
using Monopoly.DomainLayer.ReadyRoom.Events;
using Server.Common;
using SharedLibrary;

namespace Server.Hubs.ReadyRoom.EventHandlers;

public sealed class PlayerReadyEventHandler(IHubContext<ReadyRoomHub, IReadyRoomResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerReadyEvent>
{
    protected override Task HandleSpecificEvent(PlayerReadyEvent e)
    {
        return hubContext.Clients.All.PlayerReadyEvent(new PlayerReadyEventArgs(e.PlayerId, (int)e.ReadyState));
    }
}