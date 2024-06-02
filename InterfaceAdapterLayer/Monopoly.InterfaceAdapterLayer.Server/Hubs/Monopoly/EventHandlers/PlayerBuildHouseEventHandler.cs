using Microsoft.AspNetCore.SignalR;
using Monopoly.DomainLayer.Domain.Events;
using Monopoly.InterfaceAdapterLayer.Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Monopoly.InterfaceAdapterLayer.Server.Hubs.Monopoly.EventHandlers;

public class PlayerBuildHouseEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerBuildHouseEvent>
{
    protected override Task HandleSpecificEvent(PlayerBuildHouseEvent e)
    {
        return hubContext.Clients.All.PlayerBuildHouseEvent(
            new PlayerBuildHouseEventArgs
            {
                PlayerId = e.PlayerId,
                LandId = e.LandId,
                PlayerMoney = e.PlayerMoney,
                HouseCount = e.HouseCount
            }
        );
    }
}