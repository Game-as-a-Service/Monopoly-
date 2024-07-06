namespace Monopoly.DomainLayer.Common;

public abstract class Entity(string id)
{
    public string Id { get; } = id;
    private readonly List<DomainEvent> _domainEvents = [];
    public IReadOnlyList<DomainEvent> DomainEvents => _domainEvents;

    protected void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
    
    protected void AddDomainEvent(IEnumerable<DomainEvent> domainEvents)
    {
        _domainEvents.AddRange(domainEvents);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}