namespace SharedLibrary.ResponseArgs.Monopoly;

public record PlayerBidEventArgs
{
    public required string PlayerId { get; init; }
    public required string LandId { get; init; }
    public required decimal HighestPrice { get; init; }
}