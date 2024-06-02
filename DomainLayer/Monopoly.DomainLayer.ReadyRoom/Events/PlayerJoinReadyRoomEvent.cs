using Monopoly.DomainLayer.Common;

namespace Monopoly.DomainLayer.ReadyRoom.Events;

public record PlayerJoinReadyRoomEvent(string PlayerId) : DomainEvent;