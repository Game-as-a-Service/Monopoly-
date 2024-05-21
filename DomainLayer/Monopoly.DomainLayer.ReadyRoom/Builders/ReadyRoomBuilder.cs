using Monopoly.DomainLayer.ReadyRoom.Common;

namespace Monopoly.DomainLayer.ReadyRoom.Builders;

public sealed class ReadyRoomBuilder
{
    private string Id { get; set; } = Guid.NewGuid().ToString();
    private List<Player> Players { get; set; } = [];
    private string HostId { get; set; } = string.Empty;
    private IGameIdProvider GameIdProvider { get; set; } = new GameIdProvider();

    public ReadyRoomBuilder WithPlayer(Player player)
    {
        Players.Add(player);
        return this;
    }
    
    public ReadyRoomBuilder WithHost(string hostId)
    {
        HostId = hostId;
        return this;
    }
    
    public ReadyRoomBuilder WithGameIdProvider(IGameIdProvider gameIdProvider)
    {
        GameIdProvider = gameIdProvider;
        return this;
    }

    public ReadyRoomAggregate Build()
    {
        return new ReadyRoomAggregate(Id, Players, HostId)
        {
            GameIdProvider = GameIdProvider
        };
    }
}