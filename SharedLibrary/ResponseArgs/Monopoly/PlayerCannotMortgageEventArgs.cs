namespace SharedLibrary.ResponseArgs.Monopoly;

public record PlayerCannotMortgageEventArgs
{
    public required string PlayerId { get; init; }
    public required string LandId { get; init; }
    public required decimal PlayerMoney { get; init; }
}