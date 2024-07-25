namespace SharedLibrary.ResponseArgs.Monopoly;

public record PlayerCannotBuildHouseEventArgs
{
    public required string PlayerId { get; init; }
    public required string LandId { get; init; }
}