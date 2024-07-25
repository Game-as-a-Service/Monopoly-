namespace SharedLibrary.ResponseArgs.Monopoly;

public record SomePlayersPreparingEventArgs
{
    public required string GameStage { get; init; }
    public required string[] PlayerIds { get; init; }
}