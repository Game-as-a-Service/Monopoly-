using Microsoft.AspNetCore.SignalR;
using Monopoly.DomainLayer.Domain.Events;
using Monopoly.InterfaceAdapterLayer.Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Monopoly.InterfaceAdapterLayer.Server.Hubs.Monopoly.EventHandlers;

public class EndRoundEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<EndRoundEvent>
{
    protected override Task HandleSpecificEvent(EndRoundEvent e)
    {
        return hubContext.Clients.All.EndRoundEvent(new EndRoundEventArgs
        {
            PlayerId = e.PlayerId,
            NextPlayerId = e.NextPlayerId
        });
    }
}