using static Client.Pages.Gaming.Entities.Map;

namespace Client.Pages.Gaming.Entities;

public class Chess 
{
    public Player Player { get; set; }
    public string CurrentBlockId { get; set; } = "Start";
    public Direction CurrentDirection { get; set; } = Direction.None;
    public int RemainingSteps { get; set; } = 0;

    public Chess(Player player)
    {
        Player = player;
    }
}