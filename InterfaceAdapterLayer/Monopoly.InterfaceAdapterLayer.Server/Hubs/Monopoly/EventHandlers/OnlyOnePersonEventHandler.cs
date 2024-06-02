using Microsoft.AspNetCore.SignalR;
using Monopoly.DomainLayer.Domain.Events;
using Monopoly.InterfaceAdapterLayer.Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Monopoly.InterfaceAdapterLayer.Server.Hubs.Monopoly.EventHandlers;

public class OnlyOnePersonEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<OnlyOnePersonEvent>
{
    protected override Task HandleSpecificEvent(OnlyOnePersonEvent e)
    {
        return hubContext.Clients.All.OnlyOnePersonEvent(new OnlyOnePersonEventArgs
        {
            GameStage = e.GameStage,
        });
    }
}