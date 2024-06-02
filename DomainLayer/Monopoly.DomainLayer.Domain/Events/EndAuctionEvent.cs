using Monopoly.DomainLayer.Common;

namespace Monopoly.DomainLayer.Domain.Events;

public record EndAuctionEvent(string PlayerId, decimal PlayerMoney, string LandId, string? OwnerId, decimal OwnerMoney)
    : DomainEvent;