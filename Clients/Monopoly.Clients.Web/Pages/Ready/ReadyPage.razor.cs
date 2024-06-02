using System.Collections.Immutable;
using Client.Options;
using Client.Pages.Enums;
using Client.Pages.Ready.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using SharedLibrary.ResponseArgs.Monopoly;
using SharedLibrary.ResponseArgs.ReadyRoom;
using SharedLibrary.ResponseArgs.ReadyRoom.Models;
using Player = Client.Pages.Ready.Entities.Player;
using ResponseRoleEnum = SharedLibrary.ResponseArgs.ReadyRoom.Models.RoleEnum;
using PageRoleEnum = Client.Pages.Enums.RoleEnum;

namespace Client.Pages.Ready;

public partial class ReadyPage
{
    public ImmutableArray<Player> Players { get; set; } = []; 
    [Parameter] public string UserId { get; set; } = string.Empty;
    [Parameter] public string RoomId { get; set; } = string.Empty;
    [Parameter, SupplyParameterFromQuery(Name = "token")]
    public string AccessToken { get; set; } = string.Empty;
    [Inject] private IOptions<MonopolyApiOptions> BackendApiOptions { get; set; } = default!;
    private ReadyRoomHubConnection Connection { get; set; } = default!;
    public Player? CurrentPlayer => Players.FirstOrDefault(x => x.Id == UserId);
    private Popup? Popup { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var baseUri = new Uri(BackendApiOptions.Value.BaseUrl);
        var url = new Uri(baseUri, $"/ready-room?gameid={RoomId}");
        var client = new HubConnectionBuilder()
            .WithUrl(url, options =>
            {
                options.AccessTokenProvider = async () => await Task.FromResult(AccessToken);
            })
            .Build();
        Connection = new ReadyRoomHubConnection(client);
        Connection.PlayerSelectLocationEventHandler += OnPlayerSelectLocationEvent;
        Connection.PlayerSelectRoleEventHandler += OnPlayerSelectRoleEvent;
        Connection.PlayerReadyEventHandler += OnPlayerReadyEvent;
        Connection.GameStartedEventHandler += OnGameStartEvent;
        await client.StartAsync();
        var readyRoomInfos = await Connection.GetReadyRoomInfos();
        Players = [
            ..readyRoomInfos.Players.Select(x =>
            {
                return new Player
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsReady = x.IsReady,
                    IsHost = readyRoomInfos.HostId == x.Id,
                    Color = x.Location switch
                    {
                        LocationEnum.None => ColorEnum.None,
                        LocationEnum.First => ColorEnum.Red,
                        LocationEnum.Second => ColorEnum.Blue,
                        LocationEnum.Third => ColorEnum.Green,
                        LocationEnum.Fourth => ColorEnum.Yellow,
                        _ => throw new ArgumentOutOfRangeException()
                    },
                    Role = x.Role switch
                    {
                        ResponseRoleEnum.None => PageRoleEnum.None,
                        ResponseRoleEnum.OldMan => PageRoleEnum.OldMan,
                        ResponseRoleEnum.Baby => PageRoleEnum.Baby,
                        ResponseRoleEnum.Dai => PageRoleEnum.Dai,
                        ResponseRoleEnum.Mei => PageRoleEnum.Mei,
                        _ => throw new ArgumentOutOfRangeException()
                    }
                };
            })
        ];
        UserId = readyRoomInfos.RequestPlayerId;
    }

    private Task OnPlayerSelectLocationEvent(PlayerSelectLocationEventArgs e)
    {
        var player = Players.First(x => x.Id == e.PlayerId);
        player.Color = (ColorEnum)e.LocationId;
        StateHasChanged();
        return Task.CompletedTask;
    }

    private Task OnPlayerSelectRoleEvent(PlayerSelectRoleEventArgs e)
    {
        var player = Players.First(x => x.Id == e.PlayerId);
        player.Role = Enum.Parse<PageRoleEnum>(e.RoleId);
        StateHasChanged();
        return Task.CompletedTask;
    }

    private Task OnPlayerReadyEvent(PlayerReadyEventArgs e)
    {
        var player = Players.First(x => x.Id == e.PlayerId);
        player.IsReady = e.IsReady;
        StateHasChanged();
        return Task.CompletedTask;
    }

    private async Task OnPlayerCannotSelectLocationEvent(PlayerCannotSelectLocationEventArgs e)
    {
        if (Popup is null)
        {
            return;
        }

        await Popup.Show(new Popup.PopupParameter
        {
            Message = "Cannot select location.",
            Delay = 500
        });
    }

    private async Task OnGameStartEvent(GameStartedEventArgs e)
    {
        if (Popup is null)
        {
            return;
        }

        await Popup.Show(new Popup.PopupParameter
        {
            Message = $"({e.GameId}) Game start!",
            Delay = 500
        });
    }
    
    private async Task OnSelectColor(ColorEnum color)
    {
        await Connection.SelectLocation(color.ToLocationEnum());
    }
    
    private async Task OnSelectRole(string role)
    {
        await Connection.SelectRole(role);
    }
    
    private async Task OnReady()
    {
        await Connection.PlayerReady();
    }
    
    private async Task OnStart()
    {
        await Connection.StartGame();
    }
}