namespace SharedLibrary.ResponseArgs.Monopoly;

public record PlayerChooseDirectionEventArgs
{
    public required string PlayerId { get; init; }
    public required string Direction { get; init; }
}