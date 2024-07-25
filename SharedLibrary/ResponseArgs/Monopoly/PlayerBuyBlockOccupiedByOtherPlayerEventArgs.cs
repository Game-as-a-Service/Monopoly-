namespace SharedLibrary.ResponseArgs.Monopoly;

public record PlayerBuyBlockOccupiedByOtherPlayerEventArgs
{
    public required string PlayerId { get; init; }
    public required string LandId { get; init; }
}