using Microsoft.AspNetCore.SignalR;
using Monopoly.DomainLayer.Domain.Events;
using Monopoly.InterfaceAdapterLayer.Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Monopoly.InterfaceAdapterLayer.Server.Hubs.Monopoly.EventHandlers;

public class PlayerBuyBlockOccupiedByOtherPlayerEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerBuyBlockOccupiedByOtherPlayerEvent>
{
    protected override Task HandleSpecificEvent(PlayerBuyBlockOccupiedByOtherPlayerEvent e)
    {
        return hubContext.Clients.All.PlayerBuyBlockOccupiedByOtherPlayerEvent(
            new PlayerBuyBlockOccupiedByOtherPlayerEventArgs
            {
                PlayerId = e.PlayerId,
                LandId = e.LandId,
            }
        );
    }
}