using Monopoly.DomainLayer.Common;

namespace Domain.Events;

public record PlayerBankruptEvent(string PlayerId) : DomainEvent;