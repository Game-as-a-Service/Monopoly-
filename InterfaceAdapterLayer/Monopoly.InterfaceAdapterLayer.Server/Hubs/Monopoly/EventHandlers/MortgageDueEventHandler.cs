using Microsoft.AspNetCore.SignalR;
using Monopoly.DomainLayer.Domain.Events;
using Monopoly.InterfaceAdapterLayer.Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Monopoly.InterfaceAdapterLayer.Server.Hubs.Monopoly.EventHandlers;

public class MortgageDueEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<MortgageDueEvent>
{
    protected override Task HandleSpecificEvent(MortgageDueEvent e)
    {
        return hubContext.Clients.All.MortgageDueEvent(
            new MortgageDueEventArgs
            {
                PlayerId = e.PlayerId,
                LandId = e.LandId
            });
    }
}