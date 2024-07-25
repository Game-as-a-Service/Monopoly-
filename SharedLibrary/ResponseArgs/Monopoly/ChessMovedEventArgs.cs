namespace SharedLibrary.ResponseArgs.Monopoly;

public record ChessMovedEventArgs
{
    public required string PlayerId { get; init; }
    public required string BlockId { get; init; }
    public required string Direction { get; init; }
    public required int RemainingSteps { get; init; }
}