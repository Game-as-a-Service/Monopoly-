namespace SharedLibrary.ResponseArgs.Monopoly;

public record PlayerTooPoorToPayTollEventArgs
{
    public required string PlayerId { get; init; }
    public required decimal PlayerMoney { get; init; }
    public required decimal Toll { get; init; }
}