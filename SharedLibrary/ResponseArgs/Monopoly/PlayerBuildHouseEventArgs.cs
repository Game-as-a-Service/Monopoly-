namespace SharedLibrary.ResponseArgs.Monopoly;

public record PlayerBuildHouseEventArgs
{
    public required string PlayerId { get; init; }
    public required string LandId { get; init; }
    public required decimal PlayerMoney { get; init; }
    public required int HouseCount { get; init; }
}