using Monopoly.DomainLayer.Common;

namespace Monopoly.DomainLayer.Domain.Events;

public record CannotGetRewardBecauseStandOnStartEvent(string PlayerId, decimal PlayerMoney) : DomainEvent;