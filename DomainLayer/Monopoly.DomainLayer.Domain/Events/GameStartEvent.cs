using Monopoly.DomainLayer.Common;

namespace Monopoly.DomainLayer.Domain.Events;

public record GameStartEvent(string GameStage, string CurrentPlayerId)
    : DomainEvent;

public record OnlyOnePersonEvent(string GameStage)
    : DomainEvent;

public record SomePlayersPreparingEvent(string GameStage, params string[] PlayerIds)
    : DomainEvent;