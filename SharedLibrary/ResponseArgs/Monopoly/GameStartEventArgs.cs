namespace SharedLibrary.ResponseArgs.Monopoly;

public record GameStartEventArgs
{
    public required string GameStage { get; init; }
    public required string CurrentPlayerId { get; init; }
}