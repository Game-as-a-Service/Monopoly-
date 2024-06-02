using Microsoft.AspNetCore.SignalR;
using Monopoly.DomainLayer.Domain.Events;
using Monopoly.InterfaceAdapterLayer.Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Monopoly.InterfaceAdapterLayer.Server.Hubs.Monopoly.EventHandlers;

public class PlayerRedeemEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerRedeemEvent>
{
    protected override Task HandleSpecificEvent(PlayerRedeemEvent e)
    {
        return hubContext.Clients.All.PlayerRedeemEvent(new PlayerRedeemEventArgs
        {
            PlayerId = e.PlayerId,
            PlayerMoney = e.PlayerMoney,
            LandId = e.LandId,
        });
    }
}