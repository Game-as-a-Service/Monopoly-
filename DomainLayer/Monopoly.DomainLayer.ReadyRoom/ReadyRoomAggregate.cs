using Monopoly.DomainLayer.Common;
using Monopoly.DomainLayer.ReadyRoom.Builders;
using Monopoly.DomainLayer.ReadyRoom.Common;
using Monopoly.DomainLayer.ReadyRoom.Enums;
using Monopoly.DomainLayer.ReadyRoom.Events;
using Monopoly.DomainLayer.ReadyRoom.Exceptions;

namespace Monopoly.DomainLayer.ReadyRoom;

public sealed class ReadyRoomAggregate(string id, List<Player> players, string hostId) : AggregateRoot(id)
{
    public static ReadyRoomBuilder Builder() => new();
    public IGameIdProvider GameIdProvider { get; set; } = new GameIdProvider();
    public IReadOnlyList<Player> Players => players;
    public string HostId => hostId;
    public string? GameId { get; private set; } 

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

        AddDomainEvent(new PlayerReadyEvent(playerId, player.IsReady));
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
        if (hostId != playerId) throw new PlayerNotHostException(playerId);
        var unreadyPlayers = players.Where(p => p.IsReady is not true && p.Id != hostId);
        if (unreadyPlayers.Any()) throw new HostCannotStartGameException();

        GameId = GameIdProvider.GetGameId();
        AddDomainEvent(new GameStartedEvent(GameId));
    }

    public void PlayerJoin(string playerId)
    {
        if (players.Count >= 4) throw new CannotJoinReadyRoomDueToRoomIsFullException();
        if (players.Any(p => p.Id == playerId)) throw new PlayerAlreadyInRoomException();

        var player = new PlayerBuilder().WithId(playerId).Build();
        players.Add(player);
        AddDomainEvent(new PlayerJoinReadyRoomEvent(playerId));
    }
}