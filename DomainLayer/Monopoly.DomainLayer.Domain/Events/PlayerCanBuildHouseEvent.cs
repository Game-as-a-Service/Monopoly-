using Monopoly.DomainLayer.Common;

namespace Monopoly.DomainLayer.Domain.Events;

public record PlayerCanBuildHouseEvent(string PlayerId, string LandId, int HouseCount, decimal UpgradePrice) : DomainEvent;