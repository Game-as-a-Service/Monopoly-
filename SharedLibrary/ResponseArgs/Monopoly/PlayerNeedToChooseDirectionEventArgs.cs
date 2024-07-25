namespace SharedLibrary.ResponseArgs.Monopoly;

public record PlayerNeedToChooseDirectionEventArgs
{
    public required string PlayerId { get; init; }
    public required string[] Directions { get; init; }

    public virtual bool Equals(PlayerNeedToChooseDirectionEventArgs? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return PlayerId == other.PlayerId && Directions.Order().SequenceEqual(other.Directions.Order());
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(PlayerId, Directions);
    }
}