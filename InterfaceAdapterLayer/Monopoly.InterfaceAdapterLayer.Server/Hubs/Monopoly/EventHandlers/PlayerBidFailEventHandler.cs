using Microsoft.AspNetCore.SignalR;
using Monopoly.DomainLayer.Domain.Events;
using Monopoly.InterfaceAdapterLayer.Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Monopoly.InterfaceAdapterLayer.Server.Hubs.Monopoly.EventHandlers;

public class PlayerBidFailEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerBidFailEvent>
{
    protected override Task HandleSpecificEvent(PlayerBidFailEvent e)
    {
        return hubContext.Clients.All.PlayerBidFailEvent(
            new PlayerBidFailEventArgs
            {
                PlayerId = e.PlayerId,
                LandId = e.LandId,
                BidPrice = e.BidPrice,
                HighestPrice = e.HighestPrice
            });
    }
}