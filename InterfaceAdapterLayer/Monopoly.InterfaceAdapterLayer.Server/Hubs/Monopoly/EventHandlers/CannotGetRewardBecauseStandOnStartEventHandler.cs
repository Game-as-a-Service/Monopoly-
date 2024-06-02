using Microsoft.AspNetCore.SignalR;
using Monopoly.DomainLayer.Domain.Events;
using Monopoly.InterfaceAdapterLayer.Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Monopoly.InterfaceAdapterLayer.Server.Hubs.Monopoly.EventHandlers;

public class CannotGetRewardBecauseStandOnStartEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<CannotGetRewardBecauseStandOnStartEvent>
{
    protected override Task HandleSpecificEvent(CannotGetRewardBecauseStandOnStartEvent e)
    {
        return hubContext.Clients.All.CannotGetRewardBecauseStandOnStartEvent(
            new CannotGetRewardBecauseStandOnStartEventArgs
            {
                PlayerId = e.PlayerId
            });
    }
}