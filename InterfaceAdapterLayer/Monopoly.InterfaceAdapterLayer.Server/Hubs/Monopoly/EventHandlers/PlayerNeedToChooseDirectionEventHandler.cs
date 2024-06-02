using Microsoft.AspNetCore.SignalR;
using Monopoly.DomainLayer.Domain.Events;
using Monopoly.InterfaceAdapterLayer.Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Monopoly.InterfaceAdapterLayer.Server.Hubs.Monopoly.EventHandlers;

public class PlayerNeedToChooseDirectionEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerNeedToChooseDirectionEvent>
{
    protected override Task HandleSpecificEvent(PlayerNeedToChooseDirectionEvent e)
    {
        return hubContext.Clients.All.PlayerNeedToChooseDirectionEvent(
            new PlayerNeedToChooseDirectionEventArgs
            {
                PlayerId = e.PlayerId,
                Directions = e.Directions
            });
    }
}