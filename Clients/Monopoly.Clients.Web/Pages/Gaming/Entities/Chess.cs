using static Client.Pages.Gaming.Entities.Map;

namespace Client.Pages.Gaming.Entities;

public class Chess 
{
    public Player Player { get; set; }
    public string CurrentBlockId { get; set; } = "Start";
    public Direction CurrentDirection { get; set; } = Direction.Right;
    public int RemainingSteps { get; set; } = 0;
    
    public int Top { get; set; } = 20;
    public int Left { get; set; }

    public Chess(Player player)
    {
        Player = player;
        Left = 20 - (4 - player.Order) * 5;
    }
    
    public void Move()
    {
        switch (CurrentDirection)
        {
            case Direction.Up:
                Top -= 156;
                break;
            case Direction.Down:
                Top += 156;
                break;
            case Direction.Left:
                Top -= 141;
                break;
            case Direction.Right:
                Left += 141;
                break;
        }
    }
}