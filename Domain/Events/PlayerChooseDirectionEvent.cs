using Monopoly.DomainLayer.Common;

namespace Domain.Events;

public record PlayerChooseDirectionEvent(string PlayerId, string Direction) : DomainEvent;