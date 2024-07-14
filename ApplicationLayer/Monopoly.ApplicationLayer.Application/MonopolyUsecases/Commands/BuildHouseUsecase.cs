using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.DomainLayer.Common;
using Monopoly.DomainLayer.Domain;

namespace Monopoly.ApplicationLayer.Application.MonopolyUsecases.Commands;

public record BuildHouseRequest(string GameId, string PlayerId)
    : GameRequest(GameId, PlayerId);

public class BuildHouseUsecase(IRepository<MonopolyAggregate> repository, IEventBus<DomainEvent> eventBus)
    : Usecase<BuildHouseRequest>
{
    public override async Task ExecuteAsync(BuildHouseRequest request, CancellationToken cancellationToken = default)
    {
        //查
        var game = await repository.FindByIdAsync(request.GameId);

        //改
        game.BuildHouse(request.PlayerId);

        //存
        await repository.SaveAsync(game);

        //推
        await eventBus.PublishAsync(game.DomainEvents, cancellationToken);
    }
}