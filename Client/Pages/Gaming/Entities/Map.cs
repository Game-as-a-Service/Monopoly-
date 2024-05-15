namespace Client.Pages.Gaming.Entities;

public class Map
{
    private readonly Block?[][] _blocks;

    public Map(string id, Block?[][] blocks)
    {
        Id = id;
        _blocks = blocks;
        GererateBlockConnection(blocks);
    }
    public string Id { get; init; }

    public Block?[][] Blocks => _blocks;

    private static void GererateBlockConnection(Block?[][] blocks)
    {
        for (int i = 0; i < blocks.Length; i++)
        {
            for (int j = 0; j < blocks[i].Length; j++)
            {
                var block = blocks[i][j];
                if (block is null) continue;

                if (i > 0)
                {
                    block.Up = blocks[i - 1][j];
                }
                if (i < blocks.Length - 1)
                {
                    block.Down = blocks[i + 1][j];
                }
                if (j > 0)
                {
                    block.Left = blocks[i][j - 1];
                }
                if (j < blocks[i].Length - 1)
                {
                    block.Right = blocks[i][j + 1];
                }
            }
        }
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        None
    }
}