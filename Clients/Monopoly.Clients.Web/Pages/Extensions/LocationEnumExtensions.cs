using Client.Pages.Enums;
using SharedLibrary.ResponseArgs.ReadyRoom.Models;

namespace Client.Pages.Extensions;

public static class LocationEnumExtensions
{
    public static LocationEnum ToLocationEnum(this ColorEnum color)
    {
        return color switch
        {
            ColorEnum.None => LocationEnum.None,
            ColorEnum.Red => LocationEnum.First,
            ColorEnum.Blue => LocationEnum.Second,
            ColorEnum.Green => LocationEnum.Third,
            ColorEnum.Yellow => LocationEnum.Fourth,
            _ => throw new ArgumentOutOfRangeException(nameof(color), color, null)
        };
    }

    public static ColorEnum ToColorEnum(this LocationEnum location)
    {
        return location switch
        {
            LocationEnum.None => ColorEnum.None,
            LocationEnum.First => ColorEnum.Red,
            LocationEnum.Second => ColorEnum.Blue,
            LocationEnum.Third => ColorEnum.Green,
            LocationEnum.Fourth => ColorEnum.Yellow,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}