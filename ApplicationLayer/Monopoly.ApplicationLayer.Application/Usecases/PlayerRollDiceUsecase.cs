using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.DomainLayer.Common;

namespace Monopoly.ApplicationLayer.Application.Usecases;

public record PlayerRollDiceRequest(string GameId, string PlayerId)
    : GameRequest(GameId, PlayerId);

public record PlayerRollDiceResponse(IReadOnlyList<DomainEvent> Events) : CommandResponse(Events);

public class PlayerRollDiceUsecase(ICommandRepository repository, IEventBus<DomainEvent> eventBus)
    : Usecase<PlayerRollDiceRequest, PlayerRollDiceResponse>
{
    public override async Task ExecuteAsync(PlayerRollDiceRequest request,
        IPresenter<PlayerRollDiceResponse> presenter, CancellationToken cancellationToken = default)
    {
        //查
        var game = repository.FindGameById(request.GameId);

        //改
        game.PlayerRollDice(request.PlayerId);

        //存
        repository.Save(game);

        //推
        await presenter.PresentAsync(new PlayerRollDiceResponse(game.DomainEvents), cancellationToken);
    }
}