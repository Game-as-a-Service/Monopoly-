using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.DomainLayer.Common;
using Monopoly.DomainLayer.Domain;

namespace Monopoly.ApplicationLayer.Application.MonopolyUsecases.Commands;

public record SettlementRequest(string GameId, string PlayerId)
    : GameRequest(GameId, PlayerId);


public class SettlementUsecase(IRepository<MonopolyAggregate> repository, IEventBus<DomainEvent> eventBus)
    : Usecase<SettlementRequest>
{
    public override async Task ExecuteAsync(SettlementRequest request,
        CancellationToken cancellationToken = default)
    {
        //查
        var game = await repository.FindByIdAsync(request.GameId);

        //改
        game.Settlement();

        //存
        await repository.SaveAsync(game);

        //推
        await eventBus.PublishAsync(game.DomainEvents, cancellationToken);
    }
}