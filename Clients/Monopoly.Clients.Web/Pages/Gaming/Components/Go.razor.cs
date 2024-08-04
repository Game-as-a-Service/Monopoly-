using Microsoft.AspNetCore.Components;

namespace Client.Pages.Gaming.Components;

public partial class Go
{
    [Parameter, EditorRequired]
    public EventCallback OnRolledDice { get; set; }
}
