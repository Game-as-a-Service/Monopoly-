namespace SharedLibrary.ResponseArgs.Monopoly;

public record PlayerCanBuildHouseEventArgs
{
    public required string PlayerId { get; init; }
    public required string LandId { get; init; }
    public required int HouseCount { get; init; }
    public required decimal Price { get; init; }
}