using Microsoft.AspNetCore.SignalR;
using Monopoly.DomainLayer.Domain.Events;
using Monopoly.InterfaceAdapterLayer.Server.Common;
using SharedLibrary;
using PlayerRolledDiceEventArgs = SharedLibrary.ResponseArgs.Monopoly.PlayerRolledDiceEventArgs;

namespace Monopoly.InterfaceAdapterLayer.Server.Hubs.Monopoly.EventHandlers;

public class PlayerRollDiceEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext) 
    : MonopolyEventHandlerBase<PlayerRolledDiceEvent>
{
    protected override Task HandleSpecificEvent(PlayerRolledDiceEvent e)
    {
        return hubContext.Clients.All.PlayerRolledDiceEvent(new PlayerRolledDiceEventArgs
        {
            PlayerId = e.PlayerId,
            DiceCount = e.DiceCount
        });
    }
}