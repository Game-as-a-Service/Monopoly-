using Microsoft.AspNetCore.Components;

namespace Client.Pages.Gaming.Components;

public partial class DiceBox
{
    [CascadingParameter] public GamingPage Parent { get; set; } = default!;

    public List<int> ShowDice => Parent.ShowDice;

    public List<int> GetShowDices()
    {
        return ShowDice.TrueForAll(x => x > 0) ? ShowDice : [];
    }
}
