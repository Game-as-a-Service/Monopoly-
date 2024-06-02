using Monopoly.DomainLayer.Common;

namespace Monopoly.DomainLayer.Domain.Events;

public record PlayerRolledDiceEvent(string PlayerId, int DiceCount) : DomainEvent;