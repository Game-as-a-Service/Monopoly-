namespace SharedLibrary.ResponseArgs.Monopoly;

public class PlayerSelectLocationEventArgs : EventArgs
{
    public required string PlayerId { get; init; }
    public required int LocationId { get; init; }
}