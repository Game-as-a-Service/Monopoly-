namespace SharedLibrary.ResponseArgs.Monopoly;

public record PlayerBidFailEventArgs
{
    public required string PlayerId { get; init; }
    public required string LandId { get; init; }
    public required decimal BidPrice { get; init; }
    public required decimal HighestPrice { get; init; }
}