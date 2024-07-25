namespace SharedLibrary.ResponseArgs.Monopoly;

public record PlayerCanBuyLandEventArgs
{
    public required string PlayerId { get; init; }
    public required string LandId { get; init; }
    public required decimal Price { get; init; }
}