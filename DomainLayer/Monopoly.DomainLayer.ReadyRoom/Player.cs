using Monopoly.DomainLayer.Common;
using Monopoly.DomainLayer.ReadyRoom.Builders;
using Monopoly.DomainLayer.ReadyRoom.Common;
using Monopoly.DomainLayer.ReadyRoom.Enums;
using Monopoly.DomainLayer.ReadyRoom.Exceptions;

namespace Monopoly.DomainLayer.ReadyRoom;

public sealed class Player(string id, string name, LocationEnum location, string roleId, ReadyStateEnum readyState) : Entity(id)
{
    public PlayerBuilder Builder => new();
    public ReadyStateEnum ReadyState => readyState;
    public string Name => name;
    public bool IsReady => readyState == ReadyStateEnum.Ready;
    public string RoleId => roleId;
    public LocationEnum Location => location;
    

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
        if (readyState == ReadyStateEnum.Ready) throw new PlayerCannotSelectRoleDueToAlreadyReadyException();
        roleId = id;
    }

    public void SelectLocation(IReadOnlyCollection<Player> players, LocationEnum locationEnum)
    {
        if (readyState == ReadyStateEnum.Ready) throw new PlayerCannotSelectLocationDueToAlreadyReadyException();
        if (players.Any(p => p.Location == locationEnum)) throw new PlayerCannotSelectLocationDueToOccupiedException();
        location = locationEnum;
    }
}