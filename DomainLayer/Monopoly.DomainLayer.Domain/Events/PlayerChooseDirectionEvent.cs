using Monopoly.DomainLayer.Common;

namespace Monopoly.DomainLayer.Domain.Events;

public record PlayerChooseDirectionEvent(string PlayerId, string Direction) : DomainEvent;