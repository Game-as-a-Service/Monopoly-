namespace SharedLibrary.ResponseArgs.Monopoly;

public record CurrentPlayerCannotBidEventArgs
{
    public required string PlayerId { get; init; }
}