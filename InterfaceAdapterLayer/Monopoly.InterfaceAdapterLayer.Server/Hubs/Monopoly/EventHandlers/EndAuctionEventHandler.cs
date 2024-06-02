using Microsoft.AspNetCore.SignalR;
using Monopoly.DomainLayer.Domain.Events;
using Monopoly.InterfaceAdapterLayer.Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Monopoly.InterfaceAdapterLayer.Server.Hubs.Monopoly.EventHandlers;

public class EndAuctionEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<EndAuctionEvent>
{
    protected override Task HandleSpecificEvent(EndAuctionEvent e)
    {
        return hubContext.Clients.All.EndAuctionEvent(
            new EndAuctionEventArgs
            {
                PlayerId = e.PlayerId,
                LandId = e.LandId,
                OwnerId = e.OwnerId,
                PlayerMoney = e.PlayerMoney,
                OwnerMoney = e.OwnerMoney
            });
    }
}