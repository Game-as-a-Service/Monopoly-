namespace SharedLibrary.ResponseArgs.Monopoly;

public record PlayerJoinGameEventArgs
{
    public required string PlayerId { get; init; }
}