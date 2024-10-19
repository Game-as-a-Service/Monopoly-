using Monopoly.DomainLayer.Common;

namespace Monopoly.DomainLayer.Domain.Events;

public record PlayerNeedToChooseDirectionEvent(string PlayerId, string[] Directions) : DomainEvent
{
    public override string ToString()
    {
        return $"{nameof(PlayerNeedToChooseDirectionEvent)} {{ PlayerId = {PlayerId}, Directions = [{string.Join(", ", Directions)}] }}";
    }

    public virtual bool Equals(PlayerNeedToChooseDirectionEvent? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return PlayerId == other.PlayerId && Directions.Order().SequenceEqual(other.Directions.Order());
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), PlayerId, Directions);
    }
}