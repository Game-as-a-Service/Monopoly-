using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.DomainLayer.Common;
using Monopoly.DomainLayer.Domain;

namespace Monopoly.ApplicationLayer.Application.MonopolyUsecases.Commands;

public record ChooseDirectionRequest(string GameId, string PlayerId, string Direction)
    : GameRequest(GameId, PlayerId);

public class ChooseDirectionUsecase(IRepository<MonopolyAggregate> repository, IEventBus<DomainEvent> eventBus)
    : Usecase<ChooseDirectionRequest>
{
    public override async Task ExecuteAsync(ChooseDirectionRequest request
        , CancellationToken cancellationToken = default)
    {
        //查
        var game = await repository.FindByIdAsync(request.GameId);

        //改
        game.PlayerSelectDirection(request.PlayerId, request.Direction);

        //存
        await repository.SaveAsync(game);

        //推
        await eventBus.PublishAsync(game.DomainEvents, cancellationToken);
    }
}