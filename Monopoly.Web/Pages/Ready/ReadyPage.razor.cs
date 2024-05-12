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

    protected override async Task OnInitializedAsync()
    {
        Connection.GetReadyInfoEventHandler += OnGetReadyInfoEvent;
        Connection.PlayerSelectLocationEventHandler += OnPlayerSelectLocationEvent;
        Connection.PlayerSelectRoleEventHandler += OnPlayerSelectRoleEvent;
        await Connection.GetReadyInfo();
    }

    public void Update() => StateHasChanged();

    private void OnGetReadyInfoEvent(GetReadyInfoEventArgs e)
    {
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
    }

    private void OnPlayerSelectLocationEvent(PlayerSelectLocationEventArgs e)
    {
        var player = Players.FirstOrDefault(x => x.Id == e.PlayerId);
        if (player is null)
        {
            return;
        }

        player.Color = (ColorEnum)e.LocationId;
        Update();
    }

    private void OnPlayerSelectRoleEvent(PlayerSelectRoleEventArgs e)
    {
        var player = Players.FirstOrDefault(x => x.Id == e.PlayerId);
        if (player is null)
        {
            return;
        }

        player.Role = Enum.Parse<RoleEnum>(e.RoleId);
        Update();
    }
}