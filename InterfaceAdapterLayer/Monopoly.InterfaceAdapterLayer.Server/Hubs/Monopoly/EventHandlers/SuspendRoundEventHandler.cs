using Microsoft.AspNetCore.SignalR;
using Monopoly.DomainLayer.Domain.Events;
using Monopoly.InterfaceAdapterLayer.Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Monopoly.InterfaceAdapterLayer.Server.Hubs.Monopoly.EventHandlers;

public class SuspendRoundEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<SuspendRoundEvent>
{
    protected override Task HandleSpecificEvent(SuspendRoundEvent e)
    {
        return hubContext.Clients.All.SuspendRoundEvent(new SuspendRoundEventArgs
        {
            PlayerId = e.PlayerId,
            SuspendRounds = e.SuspendRounds
        });
    }
}