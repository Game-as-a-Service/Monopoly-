using Application.Common;
using Monopoly.DomainLayer.Common;

namespace Application.Usecases;

public record PlayerRollDiceRequest(string GameId, string PlayerId)
    : GameRequest(GameId, PlayerId);

public record PlayerRollDiceResponse(IReadOnlyList<DomainEvent> Events) : CommandResponse(Events);

public class PlayerRollDiceUsecase(ICommandRepository repository, IEventBus<DomainEvent> eventBus)
    : CommandUsecase<PlayerRollDiceRequest, PlayerRollDiceResponse>(repository, eventBus)
{
    public override async Task ExecuteAsync(PlayerRollDiceRequest request,
        IPresenter<PlayerRollDiceResponse> presenter, CancellationToken cancellationToken = default)
    {
        //查
        var game = Repository.FindGameById(request.GameId).ToDomain();

        //改
        game.PlayerRollDice(request.PlayerId);

        //存
        Repository.Save(game);

        //推
        await presenter.PresentAsync(new PlayerRollDiceResponse(game.DomainEvents), cancellationToken);
    }
}