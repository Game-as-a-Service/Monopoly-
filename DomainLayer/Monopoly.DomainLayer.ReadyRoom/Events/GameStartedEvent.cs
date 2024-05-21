using Monopoly.DomainLayer.ReadyRoom.Common;

namespace Monopoly.DomainLayer.ReadyRoom.Events;

public record GameStartedEvent(string GameId) : DomainEvent;