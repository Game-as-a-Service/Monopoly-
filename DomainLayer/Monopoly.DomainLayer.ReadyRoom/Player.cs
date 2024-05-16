using Monopoly.DomainLayer.ReadyRoom.Builders;
using Monopoly.DomainLayer.ReadyRoom.Common;
using Monopoly.DomainLayer.ReadyRoom.Enums;
using Monopoly.DomainLayer.ReadyRoom.Events;

namespace Monopoly.DomainLayer.ReadyRoom;

public class Player(string id, LocationEnum location, string roleId) : Entity(id)
{
    public PlayerBuilder Builder => new();

    public void Ready()
    {
    }
}