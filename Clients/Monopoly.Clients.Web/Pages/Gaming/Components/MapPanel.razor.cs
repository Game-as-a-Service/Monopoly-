using Client.Pages.Gaming.Entities;
using Microsoft.AspNetCore.Components;

namespace Client.Pages.Gaming.Components;

public partial class MapPanel
{
	[CascadingParameter] public GamingPage Parent { get; set; } = default!;

	private Block?[][] Blocks  => Parent.Map.Blocks;
	private int rowSize => Blocks.Length-1;
	private int colSize => Blocks[0].Length-1;

	private static string cssScope = "b-9s5izdb9cc";

	public int GetMapRow()
	{
		return Blocks.Length;
	}

	public int GetMapCol()
	{
		return Blocks[0].Length;
	}

	public string GetBlockTag(int row, int col)
	{
        switch (Blocks[row][col])
        {
            case null:
				if (col == 0)
				{
					return string.Format("<div class='roadLeft' {0}></div>", cssScope);
				}
				else
				{
                    return string.Format("<div class='empty' {0}></div>", cssScope);
                }
            case StartPoint:
            case ParkingLot:
            case Jail:
                return Blocks[row][col]!.GetHtml(cssScope);
			case Road:
            case Station: 
			case Land:
				return Blocks[row][col]!.GetHtml(cssScope, row, col, rowSize, colSize);
        }
        return "";
	}
}

