using Monopoly.DomainLayer.Common;

namespace Monopoly.DomainLayer.Domain.Events;

public record ChessMovedEvent(string PlayerId, string BlockId, string Direction, int RemainingSteps) : DomainEvent;