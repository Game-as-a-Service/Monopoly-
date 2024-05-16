using Monopoly.DomainLayer.ReadyRoom.Common;
using Monopoly.DomainLayer.ReadyRoom.Enums;

namespace Monopoly.DomainLayer.ReadyRoom.Events;

public sealed record PlayerReadyEvent(string PlayerId, ReadyStateEnum ReadyState, ReadyRoomAggregate ReadyRoom) : DomainEvent(ReadyRoom);