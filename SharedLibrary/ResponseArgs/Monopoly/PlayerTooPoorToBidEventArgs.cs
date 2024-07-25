namespace SharedLibrary.ResponseArgs.Monopoly;

public record PlayerTooPoorToBidEventArgs
{
    public required string PlayerId { get; init; }
    public required decimal PlayerMoney { get; init; }
    public required decimal BidPrice { get; init; }
    public required decimal HighestPrice { get; init; }
}