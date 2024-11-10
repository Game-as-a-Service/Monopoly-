using Client.Pages.Enums;
using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Player = Client.Pages.Ready.Entities.Player;

namespace Client.Pages.Ready.Components;

public partial class ColorChoicePanel
{
    [Parameter, EditorRequired]
    public required ImmutableArray<Player> Players { get; set; }
    
    [Parameter, EditorRequired]
    public required Player? CurrentPlayer { get; set; }
    
    [Parameter, EditorRequired]
    public required EventCallback<ColorEnum> OnSelectColor { get; set; }

    private string GetChoiceWrapperCss(ColorEnum color)
    {
        var player = GetPlayerWithColor(color);
        return player is null
            ? string.Empty
            : $"color-selected {(color == CurrentPlayer?.Color ? "current-player" : string.Empty)}";
    }

    private static string GetReadySignCss(Player? player)
    {
        if (player is null)
        {
            return string.Empty;
        }

        return player.IsReady ? "ready" : string.Empty;
    }

    private Player? GetPlayerWithColor(ColorEnum color)
    {
        return Players.FirstOrDefault(p => p.Color == color);
    }
}