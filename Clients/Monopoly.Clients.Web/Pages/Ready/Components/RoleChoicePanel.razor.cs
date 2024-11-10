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