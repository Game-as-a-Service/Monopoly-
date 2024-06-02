using Microsoft.AspNetCore.SignalR;
using Monopoly.DomainLayer.Domain.Events;
using Monopoly.InterfaceAdapterLayer.Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Monopoly.InterfaceAdapterLayer.Server.Hubs.Monopoly.EventHandlers;

public class PlayerNeedsToPayTollEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerNeedsToPayTollEvent>
{
    protected override Task HandleSpecificEvent(PlayerNeedsToPayTollEvent e)
    {
        return hubContext.Clients.All.PlayerNeedsToPayTollEvent(new PlayerNeedsToPayTollEventArgs
        { 
            PlayerId = e.PlayerId,
            OwnerId = e.OwnerId,
            Toll = e.Toll
        });
    }
}