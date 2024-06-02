using Monopoly.DomainLayer.Common;

namespace Monopoly.DomainLayer.Domain.Events;

public record PlayerBankruptEvent(string PlayerId) : DomainEvent;