using Monopoly.DomainLayer.Common;

namespace Monopoly.DomainLayer.Domain.Events;

public record PlayerCanBuyLandEvent(string PlayerId, string LandId, decimal Price)
    : DomainEvent;