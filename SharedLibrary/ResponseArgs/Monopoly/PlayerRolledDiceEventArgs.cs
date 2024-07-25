namespace SharedLibrary.ResponseArgs.Monopoly;

public record PlayerRolledDiceEventArgs
{   
    public required string PlayerId { get; init; }
    public required int DiceCount { get; init; }
}