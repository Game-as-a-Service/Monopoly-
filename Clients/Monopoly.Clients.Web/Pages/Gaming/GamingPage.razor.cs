﻿using Client.Components;
using Client.Options;
using Client.Pages.Extensions;
using Client.Pages.Gaming.Components;
using Client.Pages.Gaming.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using SharedLibrary.ResponseArgs.Monopoly;
using SharedLibrary.ResponseArgs.ReadyRoom.Models;
using Map = Client.Pages.Gaming.Entities.Map;
using Player = Client.Pages.Gaming.Entities.Player;
using RoleEnum = Client.Pages.Enums.RoleEnum;

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

    [Parameter, SupplyParameterFromQuery(Name = "gameid")]
    public string GameId { get; set; } = string.Empty;

    [Parameter, SupplyParameterFromQuery(Name = "token")]
    public string AccessToken { get; set; } = string.Empty;

    private GamingHubConnection Connection { get; set; } = default!;
    public IEnumerable<Player> Players { get; set; } = [];
    private string CurrentPlayerId { get; set; } = string.Empty;

    private Player _activePlayer = new();
    
    private bool IsYourTurn => CurrentPlayerId == _activePlayer.Id;
    private bool HaveRolledDice { get; set; } = false;

    private DiceBox DiceBox { get; set; } = default!;
    
    private PopupComponent Popup { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        Map = new Map("1", Blocks);
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
        Connection.ChessMovedEventHandler += ChessMovedEvent;
        Connection.PlayerNeedToChooseDirectionEventHandler += ChooseDirectionEvent;
        await client.StartAsync();

        var monopolyInfos = await Connection.GetMonopolyInfos();

        Players = monopolyInfos.Players.Select((p, index) => new Player
        {
            Id = p.Id,
            Name = p.Name,
            Money = 1000,
            Color = Enum.Parse<LocationEnum>(p.Color).ToColorEnum(),
            Role = Enum.Parse<RoleEnum>(p.Role),
            Order = index + 1
        }).ToList();
        
        CurrentPlayerId = monopolyInfos.CurrentPlayerId;
        _activePlayer = Players.First(x => x.Id == monopolyInfos.WhoAmI);
    }

    private async Task OnRolledDiceEvent(PlayerRolledDiceEventArgs e)
    {
        await DiceBox.ShowDices(e.DicePoints);
        await Popup.ShowAsync(new PopupComponent.PopupParameter
        {
            Delay = 2000,
            Message = $"{e.PlayerId} 擲出了 {e.DicePoints.Sum()}",
        });
    }

    private async Task OnRolledDice()
    {
        HaveRolledDice = true;
        await Connection.PlayerRollDice();
    }

    private async Task ChessMovedEvent(ChessMovedEventArgs e)
    {
        await Popup.ShowAsync(new PopupComponent.PopupParameter
        {
            Delay = 2000,
            Message = $"{e.PlayerId} 移動到了 {e.BlockId}",
        });
        var player = Players.First(x => x.Id == e.PlayerId);
        
        player.Chess.Move();
        player.Chess.CurrentDirection = Enum.Parse<Map.Direction>(e.Direction);
        player.Chess.CurrentBlockId = e.BlockId;
        player.Chess.RemainingSteps = e.RemainingSteps;
        
        StateHasChanged();
    }

    private Task ChooseDirectionEvent(PlayerNeedToChooseDirectionEventArgs e)
    {
        var player = Players.First(x => x.Id == e.PlayerId);
        
        StateHasChanged();
        return Task.CompletedTask;
    }
}