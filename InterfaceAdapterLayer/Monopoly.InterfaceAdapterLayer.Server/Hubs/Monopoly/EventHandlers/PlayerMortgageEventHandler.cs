using Microsoft.AspNetCore.SignalR;
using Monopoly.DomainLayer.Domain.Events;
using Monopoly.InterfaceAdapterLayer.Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Monopoly.InterfaceAdapterLayer.Server.Hubs.Monopoly.EventHandlers;

public class PlayerMortgageEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerMortgageEvent>
{
    protected override Task HandleSpecificEvent(PlayerMortgageEvent e)
    {
        return hubContext.Clients.All.PlayerMortgageEvent(new PlayerMortgageEventArgs
        {
            PlayerId = e.PlayerId,
            LandId = e.BlockId,
            PlayerMoney = e.PlayerMoney,
            DeadLine = e.DeadLine,
        });
    }
}