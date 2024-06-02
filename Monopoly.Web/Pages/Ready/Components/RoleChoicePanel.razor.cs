using Client.Pages.Enums;
using Client.Pages.Ready.Entities;
using Microsoft.AspNetCore.Components;

namespace Client.Pages.Ready.Components;

public partial class RoleChoicePanel
{
    [Parameter, EditorRequired]
    public Player? CurrentPlayer { get; set; }
    
    [Parameter, EditorRequired]
    public EventCallback<string> OnSelectRole { get; set; }
}

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
