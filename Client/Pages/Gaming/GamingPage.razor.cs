using Client.Pages.Enums;
using Client.Pages.Gaming.Entities;

namespace Client.Pages.Gaming;

public partial class GamingPage
{
    public IEnumerable<Player> Players { get; set; } = [];


    protected override async Task OnInitializedAsync()
    {
        //假資料
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

        //初始化遊戲介面，從Ready轉入Player資料

    }
}
