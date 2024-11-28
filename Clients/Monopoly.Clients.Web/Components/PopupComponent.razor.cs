using Microsoft.AspNetCore.Components;

namespace Client.Components;

public partial class PopupComponent : ComponentBase
{
    private PopupParameter? Parameter { get; set; }
    private bool IsVisible { get; set; }
    
    public async Task Show(PopupParameter parameter)
    {
        Parameter = parameter;
        IsVisible = true;
        StateHasChanged();
        await Task.Delay(parameter.Delay);
        IsVisible = false;
        StateHasChanged();
    }
    
    public class PopupParameter
    {
        public required string Message { get; init; }
        public required int Delay { get; init; }
    }
}