using Microsoft.AspNetCore.SignalR;
using Monopoly.DomainLayer.Domain.Events;
using Monopoly.InterfaceAdapterLayer.Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Monopoly.InterfaceAdapterLayer.Server.Hubs.Monopoly.EventHandlers;

public class PlayerBuyBlockEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerBuyBlockEvent>
{
    protected override Task HandleSpecificEvent(PlayerBuyBlockEvent e)
    {
        return hubContext.Clients.All.PlayerBuyBlockEvent(
            new PlayerBuyBlockEventArgs
            {
                PlayerId = e.PlayerId,
                LandId = e.LandId
            });
    }
}