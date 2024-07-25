namespace SharedLibrary.ResponseArgs.Monopoly;

public record PlayerRedeemEventArgs
{
    public required string PlayerId { get; init; }
    public required decimal PlayerMoney { get; init; }
    public required string LandId { get; init; }
}