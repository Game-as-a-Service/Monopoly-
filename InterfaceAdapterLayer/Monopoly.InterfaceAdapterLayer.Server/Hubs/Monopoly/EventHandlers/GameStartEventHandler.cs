using Microsoft.AspNetCore.SignalR;
using Monopoly.DomainLayer.Domain.Events;
using Monopoly.InterfaceAdapterLayer.Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Monopoly.InterfaceAdapterLayer.Server.Hubs.Monopoly.EventHandlers;

public class GameStartEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<GameStartEvent>
{
    protected override Task HandleSpecificEvent(GameStartEvent e)
    {
        return hubContext.Clients.All.GameStartEvent(new GameStartEventArgs
        {
            GameStage = e.GameStage,
            CurrentPlayerId = e.CurrentPlayerId,
        });
    }
}