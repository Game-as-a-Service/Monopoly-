using Monopoly.DomainLayer.Common;

namespace Monopoly.DomainLayer.Domain.Events;

public record PlayerChooseInvalidDirectionEvent(string PlayerId, string Direction) : DomainEvent;
