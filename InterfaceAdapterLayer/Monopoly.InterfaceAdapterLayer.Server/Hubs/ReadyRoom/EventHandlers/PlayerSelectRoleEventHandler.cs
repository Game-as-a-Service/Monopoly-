using Microsoft.AspNetCore.SignalR;
using Monopoly.DomainLayer.ReadyRoom.Events;
using Monopoly.InterfaceAdapterLayer.Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.ReadyRoom;

namespace Monopoly.InterfaceAdapterLayer.Server.Hubs.ReadyRoom.EventHandlers;

public class PlayerSelectRoleEventHandler(IHubContext<ReadyRoomHub, IReadyRoomResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerRoleSelectedEvent>
{
    protected override Task HandleSpecificEvent(PlayerRoleSelectedEvent e)
    {
        return hubContext.Clients.All.PlayerSelectRoleEvent(new PlayerSelectRoleEventArgs(e.PlayerId, e.RoleId));
    }
}