using Client.Pages.Enums;

namespace Client.Pages.Extensions;

public static class ColorEnumExtensions
{
    public static string ToLowerCaseName(this ColorEnum color)
    {
        return color switch
        {
            ColorEnum.None => "none",
            ColorEnum.Red => "red",
            ColorEnum.Blue => "blue",
            ColorEnum.Green => "green",
            ColorEnum.Yellow => "yellow",
            _ => throw new ArgumentOutOfRangeException(nameof(color), color, null)
        };
    }
}