namespace SharedLibrary.ResponseArgs.Monopoly;

public record SuspendRoundEventArgs
{
    public required string PlayerId { get; init; }
    public required int SuspendRounds { get; init; }
}