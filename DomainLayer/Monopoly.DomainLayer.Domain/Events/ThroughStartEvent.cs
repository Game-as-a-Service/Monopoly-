using Monopoly.DomainLayer.Common;

namespace Monopoly.DomainLayer.Domain.Events;

public record ThroughStartEvent(string PlayerId, decimal GainMoney, decimal TotalMoney) : DomainEvent;