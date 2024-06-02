using Microsoft.AspNetCore.SignalR;
using Monopoly.DomainLayer.Domain.Events;
using Monopoly.InterfaceAdapterLayer.Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Monopoly.InterfaceAdapterLayer.Server.Hubs.Monopoly.EventHandlers;

public class PlayerChooseDirectionEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerChooseDirectionEvent>
{
    protected override Task HandleSpecificEvent(PlayerChooseDirectionEvent e)
    {
        return hubContext.Clients.All.PlayerChooseDirectionEvent(new PlayerChooseDirectionEventArgs
        {
            PlayerId = e.PlayerId,
            Direction = e.Direction,
        });
    }
}