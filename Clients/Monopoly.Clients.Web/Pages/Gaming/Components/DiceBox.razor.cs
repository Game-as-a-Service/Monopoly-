namespace Client.Pages.Gaming.Components;

public partial class DiceBox
{
    private IEnumerable<int> Dices { get; set; } = [];

    public async Task ShowDices(IEnumerable<int> dices)
    {
        Console.WriteLine("ShowDices");
        Dices = dices;
        StateHasChanged();
        await Task.Delay(3000);
        Dices = [];
        StateHasChanged();
    }
}