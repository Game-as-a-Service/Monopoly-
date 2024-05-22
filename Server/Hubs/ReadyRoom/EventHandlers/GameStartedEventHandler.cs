using Microsoft.AspNetCore.SignalR;
using Monopoly.DomainLayer.ReadyRoom.Events;
using Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.ReadyRoom;

namespace Server.Hubs.ReadyRoom.EventHandlers;

public sealed class GameStartedEventHandler(IHubContext<ReadyRoomHub, IReadyRoomResponses> hubContext)
    : MonopolyEventHandlerBase<GameStartedEvent>
{
    protected override Task HandleSpecificEvent(GameStartedEvent e)
    {
        return hubContext.Clients.All.GameStartedEvent(new GameStartedEventArgs(e.GameId));
    }
}