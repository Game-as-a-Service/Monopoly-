namespace SharedLibrary.ResponseArgs.Monopoly;

public record PlayerBankruptEventArgs
{
    public required string PlayerId { get; init; }
}