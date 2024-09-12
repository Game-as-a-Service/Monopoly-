using System.Collections.Immutable;
using System.IdentityModel.Tokens.Jwt;
using Client.HttpClients;
using Client.Options;
using Client.Pages.Enums;
using Client.Pages.Ready;
using Client.Pages.Ready.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;

namespace Client.Pages;

public partial class DevPage
{
    private string _tabItem = "tab1";

    private void OpenTab1() => _tabItem = "tab1";

    private void OpenTab2() => _tabItem = "tab2";

    private readonly List<DevPlayer> _players = [];
    private readonly List<DevRoom> _rooms = [];
    private string PlayerName { get; set; } = Guid.NewGuid().ToString();
    private DevPlayer? SelectedPlayer { get; set; }
    private DevPlayer? HostToCreateRoom { get; set; }
    private List<DevPlayer> PlayersToCreateRoom { get; set; } = [];
    private DevRoom? SelectedRoom { get; set; }
    private DevRoom? RoomToOpen { get; set; }

    protected override async Task OnInitializedAsync()
    {
        // Create five players
        for (var i = 0; i < 5; i++)
        {
            await CreatePlayerAsync();
        }

        for (var i = 0; i < 5; i++)
        {
            await CreateRoomWithPlayerCountAsync(i);
        }

        return;

        async Task CreateRoomWithPlayerCountAsync(int playerCount)
        {
            // Randomly set player to host
            HostToCreateRoom = _players.First();

            var skipCount = playerCount == 4 ? 1 : 0;

            // Add other players to create room
            foreach (var player in _players.Skip(skipCount).Take(playerCount))
            {
                PlayersToCreateRoom.Add(player);
            }

            // Create a room
            await CreateRoomAsync();
        }
    }

    private async Task CreatePlayerAsync()
    {
        var token = await MonopolyDevelopmentApiClient.CreateUserAsync(PlayerName);
        _players.Add(new DevPlayer(PlayerName, token));
        PlayerName = Guid.NewGuid().ToString();
    }

    private async Task CreateRoomAsync()
    {
        if (HostToCreateRoom is null)
        {
            return;
        }

        var playerIds = PlayersToCreateRoom.Select(p => p.Token.GetMonopolyPlayerId()).ToArray();
        var roomUrl = await MonopolyDevelopmentApiClient.CreateRoomAsync(HostToCreateRoom.Token, playerIds);
        var players = playerIds.Select(id => _players.Single(p => p.Token.GetMonopolyPlayerId() == id))
            .ToImmutableArray();
        _rooms.Add(new DevRoom(roomUrl, HostToCreateRoom, players));
        HostToCreateRoom = null;
        PlayersToCreateRoom.Clear();
    }

    private static string ReadyRoomUrl(DevPlayer devPlayer, DevRoom devRoom)
    {
        return $"{devRoom.Url}?token={devPlayer.Token.RawData}";
    }

    private void AddHostToCreateRoom()
    {
        if (SelectedPlayer is null)
        {
            return;
        }

        HostToCreateRoom = SelectedPlayer;
        SelectedPlayer = null;
    }

    private void AddPlayerToCreateRoom()
    {
        if (SelectedPlayer is null
            && PlayersToCreateRoom.Count is 4)
            return;

        if (SelectedPlayer is null || PlayersToCreateRoom.Contains(SelectedPlayer))
            return;

        PlayersToCreateRoom.Add(SelectedPlayer);

        SelectedPlayer = null;
    }

    private record DevPlayer(string Name, JwtSecurityToken Token);

    private record DevRoom(string Url, DevPlayer Host, ImmutableArray<DevPlayer> Players);

    private Task OpenBelow(DevRoom room)
    {
        RoomToOpen = room;
        return Task.CompletedTask;
    }

    [Inject] private IOptions<MonopolyApiOptions> BackendApiOptions { get; set; } = default!;

    private async Task AutoStartGame()
    {
        if (SelectedRoom is null)
        {
            return;
        }

        for (var index = 0; index < SelectedRoom.Players.Length; index++)
        {
            var player = SelectedRoom.Players[index];
            var connection = await CreateReadyRoomHubConnection(player, SelectedRoom.Url.Split("/")[^1]);
            var role = Enum.GetValues<RoleEnum>()[index + 1].ToString();
            await connection.SelectRole(role);
            var color = Enum.GetValues<ColorEnum>()[index + 1];
            await connection.SelectLocation(color.ToLocationEnum());
            await connection.PlayerReady();
        }

        var hostConnection = await CreateReadyRoomHubConnection(SelectedRoom.Host, SelectedRoom.Url.Split("/")[^1]);
        await hostConnection.StartGame();
    }

    private async Task<ReadyRoomHubConnection> CreateReadyRoomHubConnection(DevPlayer player, string gameId)
    {
        var baseUri = new Uri(BackendApiOptions.Value.BaseUrl);
        var url = new Uri(baseUri, $"/ready-room?gameid={gameId}");
        var client = new HubConnectionBuilder()
            .WithUrl(url,
                options => { options.AccessTokenProvider = async () => await Task.FromResult(player.Token.RawData); })
            .Build();
        var connection = new ReadyRoomHubConnection(client);
        await client.StartAsync();
        return connection;
    }
}