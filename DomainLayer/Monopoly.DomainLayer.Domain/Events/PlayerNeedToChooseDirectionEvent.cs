using Monopoly.DomainLayer.Common;

namespace Monopoly.DomainLayer.Domain.Events;

public record PlayerNeedToChooseDirectionEvent(string PlayerId, params string[] Directions) : DomainEvent;