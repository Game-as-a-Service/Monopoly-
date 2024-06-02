using Monopoly.DomainLayer.Common;
using Monopoly.DomainLayer.ReadyRoom.Common;
using Monopoly.DomainLayer.ReadyRoom.Enums;

namespace Monopoly.DomainLayer.ReadyRoom.Events;

public record PlayerLocationSelectedEvent(string PlayerId, LocationEnum Location) : DomainEvent;