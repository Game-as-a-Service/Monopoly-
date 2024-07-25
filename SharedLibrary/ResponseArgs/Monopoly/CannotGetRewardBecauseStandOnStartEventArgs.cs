namespace SharedLibrary.ResponseArgs.Monopoly;

public record CannotGetRewardBecauseStandOnStartEventArgs
{
    public required string PlayerId { get; init; }
}