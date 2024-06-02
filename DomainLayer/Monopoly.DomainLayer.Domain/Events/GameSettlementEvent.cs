using Monopoly.DomainLayer.Common;

namespace Monopoly.DomainLayer.Domain.Events;

public record GameSettlementEvent(int Rounds, params Player[] Players) : DomainEvent;
