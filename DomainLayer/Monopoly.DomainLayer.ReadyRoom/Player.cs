using Monopoly.DomainLayer.ReadyRoom.Builders;
using Monopoly.DomainLayer.ReadyRoom.Common;
using Monopoly.DomainLayer.ReadyRoom.Enums;
using Monopoly.DomainLayer.ReadyRoom.Exceptions;

namespace Monopoly.DomainLayer.ReadyRoom;

public sealed class Player(string id, LocationEnum location, string roleId, ReadyStateEnum readyState) : Entity(id)
{
    public PlayerBuilder Builder => new();
    public ReadyStateEnum ReadyState => readyState;
    public string RoleId => roleId;

    public void Ready()
    {
        readyState = readyState switch
        {
            ReadyStateEnum.NotReady when location is LocationEnum.None => throw new PlayerLocationNotSetException(),
            ReadyStateEnum.NotReady when string.IsNullOrEmpty(roleId) => throw new PlayerRoleNotSelectedException(),
            ReadyStateEnum.NotReady => ReadyStateEnum.Ready,
            ReadyStateEnum.Ready => ReadyStateEnum.NotReady,
            _ => throw new ArgumentOutOfRangeException(nameof(readyState), readyState, null)
        };
    }

    public void SelectRole(string id)
    {
        roleId = id;
    }
}