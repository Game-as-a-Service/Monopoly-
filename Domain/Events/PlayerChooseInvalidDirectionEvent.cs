using Monopoly.DomainLayer.Common;

namespace Domain.Events;

public record PlayerChooseInvalidDirectionEvent(string PlayerId, string Direction) : DomainEvent;
