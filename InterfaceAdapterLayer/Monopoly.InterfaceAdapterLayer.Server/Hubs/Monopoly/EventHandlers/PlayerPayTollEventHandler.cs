using Microsoft.AspNetCore.SignalR;
using Monopoly.DomainLayer.Domain.Events;
using Monopoly.InterfaceAdapterLayer.Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Monopoly.InterfaceAdapterLayer.Server.Hubs.Monopoly.EventHandlers;

public class PlayerPayTollEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerPayTollEvent>
{
    protected override Task HandleSpecificEvent(PlayerPayTollEvent e)
    {
        return hubContext.Clients.All.PlayerPayTollEvent(new PlayerPayTollEventArgs
        {
            PlayerId = e.PlayerId,
            PlayerMoney = e.PlayerMoney,
            OwnerId = e.OwnerId,
            OwnerMoney = e.OwnerMoney,
        });
    }
}