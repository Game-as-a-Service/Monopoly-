using Microsoft.AspNetCore.SignalR;
using Monopoly.DomainLayer.Domain.Events;
using Monopoly.InterfaceAdapterLayer.Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Monopoly.InterfaceAdapterLayer.Server.Hubs.Monopoly.EventHandlers;

public class PlayerBuyBlockInsufficientFundsEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerBuyBlockInsufficientFundsEvent>
{
    protected override Task HandleSpecificEvent(PlayerBuyBlockInsufficientFundsEvent e)
    {
        return hubContext.Clients.All.PlayerBuyBlockInsufficientFundsEvent(
            new PlayerBuyBlockInsufficientFundsEventArgs
            {
                PlayerId = e.PlayerId,
                LandId = e.LandId,
                Price = e.Price,
            }
        );
    }
}