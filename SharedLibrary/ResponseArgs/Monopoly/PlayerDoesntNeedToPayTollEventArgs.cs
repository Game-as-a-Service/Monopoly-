namespace SharedLibrary.ResponseArgs.Monopoly;

public record PlayerDoesntNeedToPayTollEventArgs
{
    public required string PlayerId { get; init; }
    public required decimal PlayerMoney { get; init; }
}