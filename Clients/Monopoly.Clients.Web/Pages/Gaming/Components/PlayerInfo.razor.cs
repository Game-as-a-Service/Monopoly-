using Client.Pages.Gaming.Entities;
using Microsoft.AspNetCore.Components;

namespace Client.Pages.Gaming.Components;

public partial class PlayerInfo
{
    [EditorRequired][Parameter] public required Player Player { get; set; }
}
