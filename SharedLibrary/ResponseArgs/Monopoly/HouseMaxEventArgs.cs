namespace SharedLibrary.ResponseArgs.Monopoly;

public record HouseMaxEventArgs
{
    public required string PlayerId { get; init; }
    public required string LandId { get; init; }
    public required int HouseCount { get; init; }
}