using Microsoft.AspNetCore.SignalR;
using Monopoly.DomainLayer.Domain.Events;
using Monopoly.InterfaceAdapterLayer.Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Monopoly.InterfaceAdapterLayer.Server.Hubs.Monopoly.EventHandlers;

public class PlayerCanBuildHouseEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerCanBuildHouseEvent>
{
    protected override Task HandleSpecificEvent(PlayerCanBuildHouseEvent e)
    {
        return hubContext.Clients.All.PlayerCanBuildHouseEvent(new PlayerCanBuildHouseEventArgs
        {
            PlayerId = e.PlayerId,
            LandId = e.LandId,
            HouseCount = e.HouseCount,
            Price = e.UpgradePrice
        });
    }
}