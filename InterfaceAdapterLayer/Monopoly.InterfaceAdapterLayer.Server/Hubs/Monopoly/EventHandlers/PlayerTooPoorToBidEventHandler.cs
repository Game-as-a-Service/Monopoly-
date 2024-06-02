using Microsoft.AspNetCore.SignalR;
using Monopoly.DomainLayer.Domain.Events;
using Monopoly.InterfaceAdapterLayer.Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Monopoly.InterfaceAdapterLayer.Server.Hubs.Monopoly.EventHandlers;

public class PlayerTooPoorToBidEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerTooPoorToBidEvent>
{
    protected override Task HandleSpecificEvent(PlayerTooPoorToBidEvent e)
    {
        return hubContext.Clients.All.PlayerTooPoorToBidEvent(
            new PlayerTooPoorToBidEventArgs
            {
                PlayerId = e.PlayerId,
                PlayerMoney = e.PlayerMoney,
                BidPrice = e.BidPrice,
                HighestPrice = e.HighestPrice
            });
    }
}