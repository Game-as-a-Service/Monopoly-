using Client.Options;
using Client.Pages.Enums;
using Client.Pages.Gaming.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Client.Pages.Gaming;

public partial class GamingPage
{
    [Inject] private IOptions<MonopolyApiOptions> BackendApiOptions { get; set; } = default!;

    public Map Map;
    //private static Block?[][] Blocks =>
    private static Block?[][] Blocks7x7 =>
    [
        [new StartPoint("Start"), new Land("A1", lot:"A"),    new Station("Station1"),    new Land("A2", lot:"A"),    new Land("A3", lot:"A"),    null,                       null],
        [new Land("F4", lot:"F"), null,                       null,                       null,                       new Land("A4", lot:"A"),    null,                       null],
        [new Station("Station4"), null,                       new Land("B5", lot:"B"),    new Land("B6", lot:"B"),    new ParkingLot("ParkingLot"),     new Land("C1", lot:"C"),    new Land("C2", lot:"C")],
        [new Land("F3", lot:"F"), null,                       new Land("B4", lot:"B"),    null,                       new Land("B1", lot:"B"),    null,                       new Land("C3", lot:"C")],
        [new Land("F2", lot:"F"), new Land("F1", lot:"F"),    new Jail("Jail"),           new Land("B3", lot:"B"),    new Land("B2", lot:"B"),    null,                       new Station("Station2")],
        [null,                    null,                       new Land("E3", lot:"E"),    null,                       null,                       null,                       new Land("D1", lot:"D")],
        [null,                    null,                       new Land("E2", lot:"E"),    new Land("E1", lot:"E"),    new Station("Station3"),    new Land("D3", lot:"D"),    new Land("D2", lot:"D")],
    ];
    private static Block?[][] Blocks =>
    //private static Block?[][] Blocks5x9 =>
    [
        [new StartPoint("Start"), new Land("A1", lot:"A"),    new Land("A2", lot:"A"),    new Station("Station1"),    new Land("A3", lot:"A"),    new Land("A4", lot:"A"),    new Land("A5", lot:"A"),        null,                           null],
        [new Land("D1", lot:"D"), null,                       null,                       null,                       null,                       null,                       new Road("R1"),                 null,                           null],
        [new Station("Station4"), null,                       null,                       new Land("B1", lot:"B"),    new Land("B2", lot:"B"),    new Land("B3", lot:"B"),    new ParkingLot("ParkingLot"),   new Land("B4", lot:"B"),        new Land("B5", lot:"B")],
        [new Land("D2", lot:"D"), new Land("D3", lot:"D"),    new Land("D4", lot:"D"),    new Jail("Jail"),           null,                       null,                       null,                           null,                           new Station("Station2")],
        [null,                    null,                       null,                       new Land("C1", lot:"C"),    new Land("C2", lot:"C"),    new Station("Station3"),    new Land("C3", lot:"C"),        new Land("C4", lot:"C"),        new Road("R2")]
    ];

    [Parameter, SupplyParameterFromQuery(Name = "gameid")] public string GameId { get; set; } = string.Empty;
    [Parameter, SupplyParameterFromQuery(Name = "token")]
    public string AccessToken { get; set; } = string.Empty;
    private GamingHubConnection Connection { get; set; } = default!;
    public IEnumerable<Player> Players { get; set; } = [];

    protected override void OnInitialized()
    {
        Map = new Map("1", Blocks);
    }

    protected override async Task OnInitializedAsync()
    {
        //玩家假資料
        Players =
        [
            new Player
            {
                Name = "Player1",
                Money = 1000,
                Color = ColorEnum.Red,
                Role = RoleEnum.Baby,
                Order = 1,

                IsHost = true
            },
            new Player
            {
                Name = "Player2",
                Money = 1000,
                Color = ColorEnum.Blue,
                Role = RoleEnum.Dai,
                Order = 2
            },
            new Player
            {
                Name = "Player3",
                Money = 1000,
                Color = ColorEnum.Green,
                Role = RoleEnum.Mei,
                Order = 3
            },
            new Player
            {
                Name = "Player4",
                Money = 1000,
                Color = ColorEnum.Yellow,
                Role = RoleEnum.OldMan,
                Order = 4
            }
        ];

        //TODO 初始化遊戲介面，從Ready轉入Player資料

        var baseUri = new Uri(BackendApiOptions.Value.BaseUrl);
        var url = new Uri(baseUri, $"/monopoly?gameid={GameId}");
        var client = new HubConnectionBuilder()
            .WithUrl(url, options =>
            {
                options.AccessTokenProvider = async () => await Task.FromResult(AccessToken);
            })
            .Build();

        Connection = new GamingHubConnection(client);
        Connection.PlayerRolledDiceEventHandler += OnRolledDiceEvent;
        await client.StartAsync();
    }

    private Task OnRolledDiceEvent(PlayerRolledDiceEventArgs e)
    {
        var player = Players.First(x => x.Id == e.PlayerId);
        StateHasChanged();
        return Task.CompletedTask;
    }

    private async Task OnRolledDice()
    {
        await Connection.PlayerRollDice();
    }
}
