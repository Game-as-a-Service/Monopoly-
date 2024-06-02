using Microsoft.AspNetCore.SignalR;
using Monopoly.DomainLayer.Domain.Events;
using Monopoly.InterfaceAdapterLayer.Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Monopoly.InterfaceAdapterLayer.Server.Hubs.Monopoly.EventHandlers;

public class PlayerCanBuyLandEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext) : MonopolyEventHandlerBase<PlayerCanBuyLandEvent>
{
    protected override Task HandleSpecificEvent(PlayerCanBuyLandEvent e)
    {
        return hubContext.Clients.All.PlayerCanBuyLandEvent(new PlayerCanBuyLandEventArgs
        {
            PlayerId = e.PlayerId,
            LandId = e.LandId,
            Price = e.Price
        });
    }
}