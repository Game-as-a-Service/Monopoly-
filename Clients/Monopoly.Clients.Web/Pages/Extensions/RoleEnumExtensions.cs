using Client.Pages.Enums;

namespace Client.Pages.Extensions;

public static class RoleEnumExtensions
{
    public static string ToLowerCaseName(this RoleEnum role)
    {
        return role switch
        {
            RoleEnum.None => "none",
            RoleEnum.OldMan => "oldman",
            RoleEnum.Baby => "baby",
            RoleEnum.Dai => "dai",
            RoleEnum.Mei => "mei",
            _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
        };
    }
}