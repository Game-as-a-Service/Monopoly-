using Domain.Events;
using Microsoft.AspNetCore.SignalR;
using Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Server.Hubs.Monopoly.EventHandlers;

public class PlayerSelectLocationEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerSelectLocationEvent>
{
    protected override Task HandleSpecificEvent(PlayerSelectLocationEvent e)
    {
        return hubContext.Clients.All.PlayerSelectLocationEvent(new PlayerSelectLocationEventArgs
        {
            PlayerId = e.PlayerId,
            LocationId = e.LocationId,
        });
    }
}