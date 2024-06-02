using Microsoft.AspNetCore.SignalR;
using Monopoly.DomainLayer.Domain.Events;
using Monopoly.InterfaceAdapterLayer.Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Monopoly.InterfaceAdapterLayer.Server.Hubs.Monopoly.EventHandlers;

public class PlayerCannotMortgageEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerCannotMortgageEvent>
{
    protected override Task HandleSpecificEvent(PlayerCannotMortgageEvent e)
    {
        return hubContext.Clients.All.PlayerCannotMortgageEvent(new PlayerCannotMortgageEventArgs
        {
            PlayerId = e.PlayerId,
            LandId = e.LandId,
            PlayerMoney = e.PlayerMoney,
        });
    }
}