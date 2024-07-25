namespace SharedLibrary.ResponseArgs.Monopoly;

public record PlayerThroughStartEventArgs
{
    public required string PlayerId { get; init; }
    public required decimal GainMoney { get; init; }
    public required decimal TotalMoney { get; init; }
}