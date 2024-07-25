namespace SharedLibrary.ResponseArgs.Monopoly;

public record PlayerJoinGameFailedEventArgs
{
    public required string Message { get; init; }
}