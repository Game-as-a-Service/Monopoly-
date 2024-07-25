namespace SharedLibrary.ResponseArgs.Monopoly;

public record EndRoundEventArgs
{
    public required string PlayerId { get; init; }
    public required string NextPlayerId { get; init; }
}