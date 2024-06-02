using Microsoft.AspNetCore.SignalR;
using Monopoly.DomainLayer.Domain.Events;
using Monopoly.InterfaceAdapterLayer.Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Monopoly.InterfaceAdapterLayer.Server.Hubs.Monopoly.EventHandlers;

public class ThroughStartEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext) : MonopolyEventHandlerBase<ThroughStartEvent>
{
    protected override Task HandleSpecificEvent(ThroughStartEvent e)
    {
        return hubContext.Clients.All.ThroughStartEvent(new PlayerThroughStartEventArgs
        {
            PlayerId = e.PlayerId,
            GainMoney = e.GainMoney,
            TotalMoney = e.TotalMoney
        });
    }
}