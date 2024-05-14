using Client.Pages.Ready.Components;
using Client.Pages.Ready.Entities;
using Microsoft.AspNetCore.Components;
using SharedLibrary.ResponseArgs.Monopoly;
using SharedLibrary.ResponseArgs.ReadyRoom;

namespace Client.Pages.Ready;

public partial class ReadyPage
{
    public IList<Player> Players { get; set; } = []; // 不要用 IEnumerable，因為會有問題(IEnumerable 會是延遲查詢)
    [Parameter] public string UserId { get; set; } = string.Empty;
    [CascadingParameter] internal TypedHubConnection Connection { get; set; } = default!;
    public Player? CurrentPlayer => Players.FirstOrDefault(x => x.Id == UserId);
    private Popup? Popup { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Connection.GetReadyInfoEventHandler += OnGetReadyInfoEvent;
        Connection.PlayerSelectLocationEventHandler += OnPlayerSelectLocationEvent;
        Connection.PlayerSelectRoleEventHandler += OnPlayerSelectRoleEvent;
        Connection.PlayerReadyEventHandler += OnPlayerReadyEvent;
        Connection.PlayerCannotSelectLocationEventHandler += OnPlayerCannotSelectLocationEvent;
        await Connection.GetReadyInfo();
    }
    public event StateHasChangedHandler? StateHasChangeda;
public delegate void StateHasChangedHandler();
    public void Update() => StateHasChanged();

    private Task OnGetReadyInfoEvent(GetReadyInfoEventArgs e)
    {
        StateHasChangeda?.Invoke();
        Players = e.Players.Select(x => new Player
        {
            Id = x.Id,
            Name = x.Name,
            IsReady = x.IsReady,
            IsHost = e.HostId == x.Id,
            Color = Enum.Parse<ColorEnum>(x.Color.ToString()),
            Role = Enum.Parse<RoleEnum>(x.Role.ToString())
        }).ToList();
        Update();
        return Task.CompletedTask;
    }

    private Task OnPlayerSelectLocationEvent(PlayerSelectLocationEventArgs e)
    {
        var player = Players.First(x => x.Id == e.PlayerId);
        player.Color = (ColorEnum)e.LocationId;
        Update();
        return Task.CompletedTask;
    }

    private Task OnPlayerSelectRoleEvent(PlayerSelectRoleEventArgs e)
    {
        var player = Players.First(x => x.Id == e.PlayerId);
        player.Role = Enum.Parse<RoleEnum>(e.RoleId);
        Update();
        return Task.CompletedTask;
    }
    
    private Task OnPlayerReadyEvent(PlayerReadyEventArgs e)
    {
        var player = Players.First(x => x.Id == e.PlayerId);
        player.IsReady = e.PlayerState == "Ready";
        Update();
        return Task.CompletedTask;
    }
    
    private async Task OnPlayerCannotSelectLocationEvent(PlayerCannotSelectLocationEventArgs e)
    {
        if (Popup is null)
        {
            return;
        }
        var popupParameter = new Popup.PopupParameter
        {
            Message = "Cannot select location.",
            Delay = 500
        };
        await Popup.Show(popupParameter);
    }
}