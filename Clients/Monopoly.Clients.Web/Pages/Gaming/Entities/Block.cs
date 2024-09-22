namespace Client.Pages.Gaming.Entities;

public abstract class Block
{
    public string Id { get; }
    internal Block? Up { get; set; }
    internal Block? Down { get; set; }
    internal Block? Left { get; set; }
    internal Block? Right { get; set; }

    public Block(string id)
    {
        Id = id;
    }

    public virtual string GetHtml(string cssScope)
    {
        return "";
    }

    public virtual string GetHtml(string cssScope, int row, int col, int rowSize, int colSize)
    {
        return "";
    }
}

public class Land : Block
{
    protected string lot;

    public string Lot => lot;

    public Land(string id, string lot) : base(id)
    {
        this.lot = lot;
    }

    public override string GetHtml(string cssScope, int row, int col, int rowSize, int colSize)
    {
        string divRoad = string.Empty;
		string imgLand = string.Empty;
		string imgRoad = string.Empty;
        if (row == 0)
        {
            divRoad = "roadTop";
            imgLand = "landH";
            if (Left is null)
            {
                imgRoad = "roadTopL";
            }
            else if (Right is null)
            {
                imgRoad = "roadTopR";
            }
            else
            {
                imgRoad = "roadH";
            }
        }
        else if (row == rowSize)
        {
            divRoad = "roadTop";
            if (Left is null)
            {
                divRoad = "roadLeft";
                imgLand = "landV";
                imgRoad = "roadBtmL";
            }
            else if (Right is null)
            {
                divRoad = "roadRight";
                imgLand = "landV";
                imgRoad = "roadBtmR";
            }
            else
            {
                imgLand = "landH";
                imgRoad = "roadH";
            }
        }
        else if (col == 0)
        {
            divRoad = "roadLeft";
            imgLand = "landV";
            // if (row == 0) 前面已判斷過
            if (Down is null)
            {
                imgRoad = "roadBtmL";
            }
            else
            {
                imgRoad = "roadV";
            }
        }
        else if (col == colSize)
        {
            if (Up is null)
            {
                divRoad = "roadTop";
                imgLand = "landH";
                imgRoad = "roadTopR";
            }
            else
            {
                divRoad = "roadRight";
                imgLand = "landV";
                imgRoad = "roadV";
            }
        }
        else
        {
            if (Left is not null && Right is not null)
            {
                divRoad = "roadTop";
                imgLand = "landH";
                imgRoad = "roadH";
            }
            else if (Down is not null && Right is not null)
            {
                divRoad = "roadTop";
                imgLand = "landH";
                imgRoad = "roadTopL";
            }
            else if (Up is not null && Left is not null)
            {
                divRoad = "roadRight";
                imgLand = "landV";
                imgRoad = "roadBtmR";
            }
            else if (Up is not null && Down is not null)
            {
                imgLand = "landV";
                imgRoad = "roadV";
                if (Left is null && Down.Left is null)
                {
                    divRoad = "roadLeft";
                }
                else if (Right is null && Down.Right is null)
                {
                    divRoad = "roadRight";
                }
                else 
                {
                    return string.Format("<div class='roadTop' {0}><img class='roadV' {0}></div>", cssScope);
                }
            }
        }
        return string.Format("<div class='{1}' {0}><div class='{2} lot-{4}' {0}></div><img class='{3}' {0}></div>", cssScope, divRoad, imgLand, imgRoad, lot);
    }
}

public class StartPoint : Block
{
    public StartPoint(string id) : base(id)
    {
    }

    public override string GetHtml(string cssScope)
    {
        return string.Format("<div class='start' {0}></div>", cssScope);
    }
}
public class Jail : Block
{
    public Jail(string id) : base(id)
    {
    }

    public override string GetHtml(string cssScope)
    {
        return string.Format("<div class='prison' {0}></div>", cssScope);
    }

}
public class ParkingLot : Block
{
    public ParkingLot(string id) : base(id)
    {
    }

    public override string GetHtml(string cssScope)
    {
        return string.Format("<div class='parking' {0}></div>", cssScope);
    }

}

public class Station : Land
{
    public Station(string id, string lot = "S") : base(id, lot)
    {
    }

    public override string GetHtml(string cssScope, int row, int col, int rowSize, int colSize)
    {
        string divRoad = string.Empty;
		string imgLand = string.Empty;
		string imgRoad = string.Empty;
        if(col == 0)
        {
            divRoad = "roadLeft";
            imgLand = "landV";
            imgRoad = "railV";
        }
        else if (col == colSize)
        {
            divRoad = "roadRight";
            imgLand = "landV";
            imgRoad = "railV";
        }
        else
        {
            divRoad = "roadTop";
            imgLand = "landH";
            imgRoad = "railH";
        }
        return string.Format("<div class='{1}' {0}><div class='{2} lot-{4}' {0}></div><img class='{3}' {0}></div>", cssScope, divRoad, imgLand, imgRoad, lot);
    }
}

public class Road : Block
{
    public Road(string id) : base(id)
    {
    }

    public override string GetHtml(string cssScope, int row, int col, int rowSize, int colSize)
    {
        string divRoad = string.Empty;
        if(row == 0)
        {
            if (col == 0)
            {
                divRoad = "pathTopL";
            }
            else if (col == colSize)
            {
                divRoad = "pathTopR";
            }
            else
            {
                divRoad = "pathH";
            }
        }
        else if (row == rowSize)
        {
            if (col == 0)
            {
                divRoad = "pathBtmL";
            }
            else if (col == colSize)
            {
                divRoad = "pathBtmR";
            }
            else
            {
                divRoad = "pathH";
            }
        }
        else if (col == 0)
        {	
            if (Up is not null && Down is not null)
            {
                divRoad = "pathV";
            }
            else if (Up is not null && Right is not null)
            {
                divRoad = "pathBtmL";
            }
            else if (Down is not null && Right is not null)
            {
                divRoad = "pathTopL";
            }
        }
        else if (col == colSize)
        {	
            if (Up is not null && Down is not null)
            {
                divRoad = "pathV";
            }
            else if (Up is not null && Left is not null)
            {
                divRoad = "pathBtmR";
            }
            else if (Down is not null && Left is not null)
            {
                divRoad = "pathTopR";
            }
        }
        else
        {
            if (Up is not null && Down is not null)
            {
                divRoad = "pathV";
            }
            else if (Left is not null && Right is not null)
            {
                divRoad = "pathH";
            }
            else if (Up is not null && Left is not null)
            {
                divRoad = "pathBtmR";
            }
            else if (Up is not null && Right is not null)
            {
                divRoad = "pathBtmL";
            }
            else if (Down is not null && Left is not null)
            {
                divRoad = "pathTopR";
            }
            else if (Down is not null && Right is not null)
            {
                divRoad = "pathTopL";
            }
        }
        return string.Format("<div class='{1}' {0}></div>", cssScope, divRoad);
    }
}