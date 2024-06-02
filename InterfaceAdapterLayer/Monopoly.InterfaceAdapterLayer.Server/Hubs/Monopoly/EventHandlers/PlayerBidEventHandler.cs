using Microsoft.AspNetCore.SignalR;
using Monopoly.DomainLayer.Domain.Events;
using Monopoly.InterfaceAdapterLayer.Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Monopoly.InterfaceAdapterLayer.Server.Hubs.Monopoly.EventHandlers;

public class PlayerBidEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerBidEvent>
{
    protected override Task HandleSpecificEvent(PlayerBidEvent e)
    {
        return hubContext.Clients.All.PlayerBidEvent(
            new PlayerBidEventArgs
            {
                PlayerId = e.PlayerId,
                LandId = e.LandId,
                HighestPrice = e.HighestPrice
            });
    }
}