using Client.Pages.Enums;

namespace Client.Pages.Gaming.Entities;

public class Player
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsHost { get; set; } = false;
    public ColorEnum Color { get; set; } = ColorEnum.None;
    public RoleEnum Role { get; set; } = RoleEnum.None;
    public int Order { get; set; } = 0;

    public int Money { get; set; } = 0;
    public IEnumerable<ILandContract> LandContracts { get; set; } = [];
    public GamingStatusEnum Status { get; set; } = GamingStatusEnum.None;

    public int GetTotalMoney => Money + LandContracts.Sum(x => x.Money + x.HouseMoney);
}
