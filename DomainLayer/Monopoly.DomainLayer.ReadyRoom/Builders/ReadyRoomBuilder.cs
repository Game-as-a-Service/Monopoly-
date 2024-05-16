namespace Monopoly.DomainLayer.ReadyRoom.Builders;

public class ReadyRoomBuilder
{
    private string Id { get; set; } = Guid.NewGuid().ToString();
    private List<Player> Players { get; set; } = [];

    public ReadyRoomBuilder WithPlayer(Player player)
    {
        Players.Add(player);
        return this;
    }

    public ReadyRoomAggregate Build()
    {
        return new ReadyRoomAggregate(Id, Players);
    }
}