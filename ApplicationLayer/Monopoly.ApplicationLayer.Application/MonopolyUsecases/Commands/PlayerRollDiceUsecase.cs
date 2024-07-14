using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.DomainLayer.Common;
using Monopoly.DomainLayer.Domain;

namespace Monopoly.ApplicationLayer.Application.MonopolyUsecases.Commands;

public record PlayerRollDiceRequest(string GameId, string PlayerId)
    : GameRequest(GameId, PlayerId);

public record PlayerRollDiceResponse(IReadOnlyList<DomainEvent> Events) : CommandResponse(Events);

public class PlayerRollDiceUsecase(IRepository<MonopolyAggregate> repository, IEventBus<DomainEvent> eventBus)
    : Usecase<PlayerRollDiceRequest, PlayerRollDiceResponse>
{
    public override async Task ExecuteAsync(PlayerRollDiceRequest request,
        IPresenter<PlayerRollDiceResponse> presenter, CancellationToken cancellationToken = default)
    {
        //查
        var game = await repository.FindByIdAsync(request.GameId);

        //改
        game.PlayerRollDice(request.PlayerId);

        //存
        await repository.SaveAsync(game);

        //推
        await presenter.PresentAsync(new PlayerRollDiceResponse(game.DomainEvents), cancellationToken);
    }
}