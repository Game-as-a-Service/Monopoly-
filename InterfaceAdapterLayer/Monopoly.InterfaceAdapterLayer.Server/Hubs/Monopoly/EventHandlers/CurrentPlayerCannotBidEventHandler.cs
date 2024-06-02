using Microsoft.AspNetCore.SignalR;
using Monopoly.DomainLayer.Domain.Events;
using Monopoly.InterfaceAdapterLayer.Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Monopoly.InterfaceAdapterLayer.Server.Hubs.Monopoly.EventHandlers;

public class CurrentPlayerCannotBidEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<CurrentPlayerCannotBidEvent>
{
    protected override Task HandleSpecificEvent(CurrentPlayerCannotBidEvent e)
    {
        return hubContext.Clients.All.CurrentPlayerCannotBidEvent(
            new CurrentPlayerCannotBidEventArgs
            {
                PlayerId = e.PlayerId
            });
    }
}