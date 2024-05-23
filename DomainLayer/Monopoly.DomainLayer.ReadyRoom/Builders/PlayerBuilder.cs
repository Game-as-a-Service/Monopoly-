using Monopoly.DomainLayer.ReadyRoom.Enums;

namespace Monopoly.DomainLayer.ReadyRoom.Builders;

public sealed class PlayerBuilder
{
    private string Id { get; set; } = Guid.NewGuid().ToString();
    
    private string Name { get; set; } = Guid.NewGuid().ToString();

    private LocationEnum Location { get; set; } = LocationEnum.None;

    private string RoleId { get; set; } = string.Empty;

    private ReadyStateEnum ReadyState { get; set; } = ReadyStateEnum.NotReady;

    public PlayerBuilder WithId(string id)
    {
        Id = id;
        return this;
    }

    public PlayerBuilder WithLocation(LocationEnum location)
    {
        Location = location;
        return this;
    }

    public PlayerBuilder WithReady()
    {
        ReadyState = ReadyStateEnum.Ready;
        return this;
    }

    public PlayerBuilder WithRole(string roleId)
    {
        RoleId = roleId;
        return this;
    }

    public Player Build()
    {
        return new Player(Id, Name, Location, RoleId, ReadyState);
    }
}