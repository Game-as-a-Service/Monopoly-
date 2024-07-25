namespace SharedLibrary.ResponseArgs.Monopoly;

public record EndRoundFailEventArgs
{
    public required string PlayerId { get; init; }
}