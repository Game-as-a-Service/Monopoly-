using Domain.Events;
using Microsoft.AspNetCore.SignalR;
using Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Server.Hubs.EventHandlers;

public class PlayerCannotSelectLocationEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerCannotSelectLocationEvent>
{
    protected override Task HandleSpecificEvent(PlayerCannotSelectLocationEvent e)
    {
        return hubContext.Clients.All.PlayerCannotSelectLocationEvent(new PlayerCannotSelectLocationEventArgs
        {
            PlayerId = e.PlayerId,
            LocationId = e.LocationId
        });
    }
}