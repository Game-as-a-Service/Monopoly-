namespace Client.Pages.Gaming.Entities;

public interface ILandContract
{
    int Money { get; set; }
    int HouseCount { get; set; }

    int HouseMoney => HouseCount * 50; //TODO
}
