using Monopoly.DomainLayer.Common;

namespace Monopoly.ApplicationLayer.Application.Common;

public abstract record Response();
public abstract record CommandResponse(IReadOnlyList<DomainEvent> Events) : Response;