using Monopoly.DomainLayer.Common;

namespace Monopoly.DomainLayer.Domain.Events;

public record PlayerHadSelectedDirectionEvent(string PlayerId) : DomainEvent;
