namespace SharedLibrary.ResponseArgs.Monopoly;

public record PlayerBuyBlockEventArgs
{
    public required string PlayerId { get; init; }
    public required string LandId { get; init; }
}