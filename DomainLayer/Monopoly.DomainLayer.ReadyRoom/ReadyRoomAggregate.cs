using Monopoly.DomainLayer.ReadyRoom.Builders;
using Monopoly.DomainLayer.ReadyRoom.Common;
using Monopoly.DomainLayer.ReadyRoom.Events;

namespace Monopoly.DomainLayer.ReadyRoom;

public sealed class ReadyRoomAggregate(string id, List<Player> players) : AggregateRoot(id)
{
    public static ReadyRoomBuilder Builder => new();

    public void PlayerReady(string playerId)
    {
        var player = GetPlayer(playerId);

        player.Ready();
        
        AddDomainEvent(new PlayerReadyEvent(playerId, player.ReadyState));
    }

    private Player GetPlayer(string playerId)
    {
        var player = players.FirstOrDefault(p => p.Id == playerId);
        if (player == null)
        {
            throw new ArgumentException("Player not found");
        }

        return player;
    }

    public void SelectRole(string playerId, string roleId)
    {
        var player = GetPlayer(playerId);

        player.SelectRole(roleId);
        
        AddDomainEvent(new PlayerRoleSelectedEvent(playerId, roleId));
    }
}