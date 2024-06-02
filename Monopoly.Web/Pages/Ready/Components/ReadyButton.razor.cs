using System.Collections.Immutable;
using Client.Pages.Enums;
using Client.Pages.Ready.Entities;
using Microsoft.AspNetCore.Components;

namespace Client.Pages.Ready.Components;

public partial class ReadyButton
{
    [Parameter, EditorRequired]
    public Player? CurrentPlayer { get; set; }
    
    [Parameter, EditorRequired]
    public required ImmutableArray<Player> Players { get; set; }
    
    [Parameter, EditorRequired]
    public EventCallback OnReady { get; set; }
    
    [Parameter, EditorRequired]
    public EventCallback OnStart { get; set; }

    private bool EnabledToReady =>
        CurrentPlayer?.Color is not ColorEnum.None && CurrentPlayer?.Role is not RoleEnum.None;
}