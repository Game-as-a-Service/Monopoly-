using Domain.Common;

namespace Domain.Events;

public record PlayerSelectLocationEvent(string PlayerId, int LocationId) : DomainEvent;
public record PlayerCannotSelectLocationEvent(string PlayerId, int LocationId)
    : DomainEvent;
