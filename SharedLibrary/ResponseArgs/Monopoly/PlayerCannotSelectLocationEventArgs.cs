namespace SharedLibrary.ResponseArgs.Monopoly;

public class PlayerCannotSelectLocationEventArgs : EventArgs
{
    public required string PlayerId { get; init; }
    public required int LocationId { get; init; }
}