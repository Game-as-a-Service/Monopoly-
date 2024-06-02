using Client.Pages.Enums;
using Client.Pages.Ready.Entities;
using Microsoft.AspNetCore.Components;

namespace Client.Pages.Ready.Components;

public partial class ReadyButton
{
    [CascadingParameter] public ReadyPage Parent { get; set; } = default!;
    private Player? CurrentPlayer => Parent.CurrentPlayer;

    private bool EnabledToReady =>
        CurrentPlayer?.Color is not ColorEnum.None && CurrentPlayer?.Role is not RoleEnum.None;

    private bool EnabledToStart =>
        EnabledToReady
        && Parent.Players
            .Where(p => p != CurrentPlayer)
            .All(p => p.IsReady);

    private async Task Ready()
    {
        if (!EnabledToReady || CurrentPlayer is null)
            return;
        await Parent.Connection.PlayerReady();
    }

    private async Task Start()
    {
        if (!EnabledToStart)
            return;
        await Parent.Connection.GameStart();
    }
}