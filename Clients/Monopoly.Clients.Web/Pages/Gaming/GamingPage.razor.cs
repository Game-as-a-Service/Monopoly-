using Client.Pages.Enums;
using Client.Pages.Gaming.Entities;

namespace Client.Pages.Gaming;

public partial class GamingPage
{
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
		[new StartPoint("Start"), new Land("A1", lot:"A"),	  new Land("A2", lot:"A"),    new Station("Station1"),    new Land("A3", lot:"A"),    new Land("A4", lot:"A"),	  new Land("A5", lot:"A"),    	  null,         				  null],
		[new Land("D1", lot:"D"), null,                       null,                       null,    					  null,                   	  null,						  new Road("R1"),    	  		  null,                       	  null],
		[new Station("Station4"), null,       				  null,               		  new Land("B1", lot:"B"),    new Land("B2", lot:"B"),	  new Land("B3", lot:"B"),    new ParkingLot("ParkingLot"),   new Land("B4", lot:"B"),		  new Land("B5", lot:"B")],
		[new Land("D2", lot:"D"), new Land("D3", lot:"D"),    new Land("D4", lot:"D"),    new Jail("Jail"),   		  null,    					  null,						  null,							  null,     					  new Station("Station2")],
		[null, 					  null,						  null,						  new Land("C1", lot:"C"), 	  new Land("C2", lot:"C"),    new Station("Station3"),    new Land("C3", lot:"C"),    	  new Land("C4", lot:"C"),   	  new Road("R2")]
	];

    public Map Map ;
    
    public IEnumerable<Player> Players { get; set; } = [];

    protected override void OnInitialized()
    {
        Map = new Map("1", Blocks);
    }
    
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
