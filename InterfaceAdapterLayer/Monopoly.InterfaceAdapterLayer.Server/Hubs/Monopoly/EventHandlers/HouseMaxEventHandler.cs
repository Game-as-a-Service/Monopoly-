using Microsoft.AspNetCore.SignalR;
using Monopoly.DomainLayer.Domain.Events;
using Monopoly.InterfaceAdapterLayer.Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Monopoly.InterfaceAdapterLayer.Server.Hubs.Monopoly.EventHandlers;

public class HouseMaxEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<HouseMaxEvent>
{
    protected override Task HandleSpecificEvent(HouseMaxEvent e)
    {
        return hubContext.Clients.All.HouseMaxEvent(
            new HouseMaxEventArgs
            {
                PlayerId = e.PlayerId,
                LandId = e.LandId,
                HouseCount = e.HouseCount
            });
    }
}