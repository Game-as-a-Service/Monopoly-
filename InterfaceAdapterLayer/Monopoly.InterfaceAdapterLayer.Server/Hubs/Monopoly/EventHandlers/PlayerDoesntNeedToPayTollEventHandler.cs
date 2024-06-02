using Microsoft.AspNetCore.SignalR;
using Monopoly.DomainLayer.Domain.Events;
using Monopoly.InterfaceAdapterLayer.Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Monopoly.InterfaceAdapterLayer.Server.Hubs.Monopoly.EventHandlers;

public class PlayerDoesntNeedToPayTollEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerDoesntNeedToPayTollEvent>
{
    protected override Task HandleSpecificEvent(PlayerDoesntNeedToPayTollEvent e)
    {
        return hubContext.Clients.All.PlayerDoesntNeedToPayTollEvent(new PlayerDoesntNeedToPayTollEventArgs
        {
            PlayerId = e.PlayerId,
            PlayerMoney = e.PlayerMoney
        });
    }
}