namespace SharedLibrary.ResponseArgs.Monopoly;

public record OnlyOnePersonEventArgs
{
    public required string GameStage { get; init; }
}