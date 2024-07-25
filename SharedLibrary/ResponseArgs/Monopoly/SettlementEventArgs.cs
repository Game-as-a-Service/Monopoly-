namespace SharedLibrary.ResponseArgs.Monopoly;

public record SettlementEventArgs
{
    public required int Rounds { get; init; }
    public required string[] PlayerIds { get; init; }
}