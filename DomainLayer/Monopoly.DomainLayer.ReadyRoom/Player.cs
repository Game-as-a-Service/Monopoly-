using Monopoly.DomainLayer.Common;
using Monopoly.DomainLayer.ReadyRoom.Builders;
using Monopoly.DomainLayer.ReadyRoom.Common;
using Monopoly.DomainLayer.ReadyRoom.Enums;
using Monopoly.DomainLayer.ReadyRoom.Exceptions;

namespace Monopoly.DomainLayer.ReadyRoom;

public sealed class Player(string id, string name, LocationEnum location, string roleId, bool isReady) : Entity(id)
{
    public PlayerBuilder Builder => new();
    public string Name => name;
    public bool IsReady => isReady;
    public string RoleId => roleId;
    public LocationEnum Location => location;
    

    public void Ready()
    {
        isReady = isReady switch
        {
            false when location is LocationEnum.None => throw new PlayerLocationNotSetException(),
            false when string.IsNullOrEmpty(roleId) => throw new PlayerRoleNotSelectedException(),
            false => true,
            true => false
        };
    }

    public void SelectRole(string id)
    {
        if (isReady) throw new PlayerCannotSelectRoleDueToAlreadyReadyException();
        roleId = id;
    }

    public void SelectLocation(IReadOnlyCollection<Player> players, LocationEnum locationEnum)
    {
        if (isReady) throw new PlayerCannotSelectLocationDueToAlreadyReadyException();
        if (players.Any(p => p.Location == locationEnum)) throw new PlayerCannotSelectLocationDueToOccupiedException();
        location = locationEnum;
    }
}