using Microsoft.AspNetCore.SignalR;
using Monopoly.DomainLayer.Domain.Events;
using Monopoly.InterfaceAdapterLayer.Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Monopoly.InterfaceAdapterLayer.Server.Hubs.Monopoly.EventHandlers;

public class PlayerCannotBuildHouseEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerCannotBuildHouseEvent>
{
    protected override Task HandleSpecificEvent(PlayerCannotBuildHouseEvent e)
    {
        return hubContext.Clients.All.PlayerCannotBuildHouseEvent(
            new PlayerCannotBuildHouseEventArgs
            {
                PlayerId = e.PlayerId,
                LandId = e.LandId
            });
    }
}