using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.DomainLayer.Common;
using Monopoly.DomainLayer.Domain;

namespace Monopoly.ApplicationLayer.Application.MonopolyUsecases.Commands;

public record PlayerBuyLandRequest(string GameId, string PlayerId, string LandID)
    : GameRequest(GameId, PlayerId);

public class PlayerBuyLandUsecase(IRepository<MonopolyAggregate> repository, IEventBus<DomainEvent> eventBus)
    : Usecase<PlayerBuyLandRequest>
{
    public override async Task ExecuteAsync(PlayerBuyLandRequest request,
        CancellationToken cancellationToken = default)
    {
        //查
        var game = await repository.FindByIdAsync(request.GameId);

        //改
        game.BuyLand(request.PlayerId, request.LandID);

        //存
        await repository.SaveAsync(game);

        //推
        await eventBus.PublishAsync(game.DomainEvents, cancellationToken);
    }
}