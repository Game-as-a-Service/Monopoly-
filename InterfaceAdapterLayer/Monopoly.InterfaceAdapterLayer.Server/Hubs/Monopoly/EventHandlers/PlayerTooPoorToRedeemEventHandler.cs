using Microsoft.AspNetCore.SignalR;
using Monopoly.DomainLayer.Domain.Events;
using Monopoly.InterfaceAdapterLayer.Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Monopoly.InterfaceAdapterLayer.Server.Hubs.Monopoly.EventHandlers;

public class PlayerTooPoorToRedeemEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerTooPoorToRedeemEvent>
{
    protected override Task HandleSpecificEvent(PlayerTooPoorToRedeemEvent e)
    {
        return hubContext.Clients.All.PlayerTooPoorToRedeemEvent(new PlayerTooPoorToRedeemEventArgs
        {
            PlayerId = e.PlayerId,
            PlayerMoney = e.PlayerMoney,
            LandId = e.BlockId,
            RedeemPrice = e.RedeemPrice,
        });
    }
}