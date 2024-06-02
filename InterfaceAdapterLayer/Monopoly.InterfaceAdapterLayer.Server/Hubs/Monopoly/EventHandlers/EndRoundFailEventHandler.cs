using Microsoft.AspNetCore.SignalR;
using Monopoly.DomainLayer.Domain.Events;
using Monopoly.InterfaceAdapterLayer.Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Monopoly.InterfaceAdapterLayer.Server.Hubs.Monopoly.EventHandlers;

public class EndRoundFailEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<EndRoundFailEvent>
{
    protected override Task HandleSpecificEvent(EndRoundFailEvent e)
    {
        return hubContext.Clients.All.EndRoundFailEvent(new EndRoundFailEventArgs
        {
            PlayerId = e.PlayerId,
        });
    }
}