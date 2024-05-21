using Monopoly.DomainLayer.ReadyRoom.Builders;
using Monopoly.DomainLayer.ReadyRoom.Common;
using Monopoly.DomainLayer.ReadyRoom.Enums;
using Monopoly.DomainLayer.ReadyRoom.Events;
using Monopoly.DomainLayer.ReadyRoom.Exceptions;

namespace Monopoly.DomainLayer.ReadyRoom;

public sealed class ReadyRoomAggregate(string id, List<Player> players, string hostId) : AggregateRoot(id)
{
    public static ReadyRoomBuilder Builder => new();
    public IGameIdProvider GameIdProvider { get; set; } = new GameIdProvider();

    private Player GetPlayer(string playerId)
    {
        var player = players.FirstOrDefault(p => p.Id == playerId);
        if (player == null) throw new ArgumentException("Player not found");

        return player;
    }

    public void PlayerReady(string playerId)
    {
        var player = GetPlayer(playerId);

        player.Ready();

        AddDomainEvent(new PlayerReadyEvent(playerId, player.ReadyState));
    }

    public void SelectRole(string playerId, string roleId)
    {
        var player = GetPlayer(playerId);

        player.SelectRole(roleId);

        AddDomainEvent(new PlayerRoleSelectedEvent(playerId, roleId));
    }

    public void SelectLocation(string playerId, LocationEnum location)
    {
        var player = GetPlayer(playerId);

        player.SelectLocation(players, location);

        AddDomainEvent(new PlayerLocationSelectedEvent(playerId, location));
    }

    public void StartGame(string playerId)
    {
        if (hostId != playerId) throw new PlayerNotHostException();
        var unreadyPlayers = players.Where(p => p.ReadyState is ReadyStateEnum.NotReady && p.Id != hostId);
        if (unreadyPlayers.Any()) throw new HostCannotStartGameException();

        var gameId = GameIdProvider.GetGameId();
        AddDomainEvent(new GameStartedEvent(gameId));
    }
}