using Microsoft.AspNetCore.SignalR;
using Monopoly.DomainLayer.Domain.Events;
using Monopoly.InterfaceAdapterLayer.Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Monopoly.InterfaceAdapterLayer.Server.Hubs.Monopoly.EventHandlers;

public class SomePlayersPreparingEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<SomePlayersPreparingEvent>
{
    protected override Task HandleSpecificEvent(SomePlayersPreparingEvent e)
    {
        return hubContext.Clients.All.SomePlayersPreparingEvent(new SomePlayersPreparingEventArgs
        {
            GameStage = e.GameStage,
            PlayerIds = e.PlayerIds,
        });
    }
}