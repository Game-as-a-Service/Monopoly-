using Microsoft.AspNetCore.SignalR;
using Monopoly.DomainLayer.Domain.Events;
using Monopoly.InterfaceAdapterLayer.Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Monopoly.InterfaceAdapterLayer.Server.Hubs.Monopoly.EventHandlers;

public class PlayerTooPoorToPayTollEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerTooPoorToPayTollEvent>
{
    protected override Task HandleSpecificEvent(PlayerTooPoorToPayTollEvent e)
    {
        return hubContext.Clients.All.PlayerTooPoorToPayTollEvent(new PlayerTooPoorToPayTollEventArgs
        {
            PlayerId = e.PlayerId,
            PlayerMoney = e.PlayerMoney,
            Toll = e.Toll,
        });
    }
}