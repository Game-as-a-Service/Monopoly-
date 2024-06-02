using System.Collections.Immutable;
using System.IdentityModel.Tokens.Jwt;
using Client.HttpClients;

namespace Client.Pages;

public partial class DevPage
{
    private List<DevPlayer> _players = [];
    private List<DevRoom> _rooms = [];
    private string PlayerName { get; set; } = Guid.NewGuid().ToString();
    private DevPlayer? SelectedPlayer { get; set; }
    private DevPlayer? HostToCreateRoom { get; set; }
    private List<DevPlayer> PlayersToCreateRoom { get; set; } = [];
    private DevRoom? SelectedRoom { get; set; }

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
        var token = await _monopolyDevelopmentApiClient.CreateUserAsync(PlayerName);
        _players.Add(new DevPlayer(PlayerName, token));
        PlayerName = Guid.NewGuid().ToString();
    }

    private async Task CreateRoomAsync()
    {
        if (HostToCreateRoom is null)
        {
            return;
        }

        var hostToken = HostToCreateRoom.Token;
        var playerIds = PlayersToCreateRoom.Select(p => p.Token.GetMonopolyPlayerId()).ToArray();
        var roomUrl = await _monopolyDevelopmentApiClient.CreateRoomAsync(hostToken, playerIds);
        var players = playerIds.Select(id => _players.Single(p => p.Token.GetMonopolyPlayerId() == id))
            .ToImmutableArray();
        _rooms.Add(new DevRoom(roomUrl, hostToken, players));
        HostToCreateRoom = null;
        PlayersToCreateRoom.Clear();
    }

    private string ReadyRoomUrl(DevPlayer devPlayer, DevRoom devRoom)
    {
        // navigate to {devRoom.Url}?token={devPlayer.Token.RowData}
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
        if (SelectedPlayer is null)
        {
            return;
        }

        if (PlayersToCreateRoom.Count == 4)
        {
            return;
        }

        if (PlayersToCreateRoom.Contains(SelectedPlayer))
        {
            return;
        }

        PlayersToCreateRoom.Add(SelectedPlayer);

        SelectedPlayer = null;
    }

    private record DevPlayer(string Name, JwtSecurityToken Token);

    private record DevRoom(string Url, JwtSecurityToken HostToken, ImmutableArray<DevPlayer> Players);
}