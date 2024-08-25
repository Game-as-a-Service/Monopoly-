namespace SharedLibrary.ResponseArgs.Monopoly;

public record PlayerRolledDiceEventArgs
{
    public required string PlayerId { get; init; }
    public required int[] DicePoints { get; init; }

    public virtual bool Equals(PlayerRolledDiceEventArgs? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return PlayerId == other.PlayerId && DicePoints.SequenceEqual(other.DicePoints);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(PlayerId, DicePoints);
    }
}