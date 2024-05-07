using Application.Common;
using Domain.Common;

namespace Application.Usecases;

public record PlayerReadyRequest(string GameId, string PlayerId)
    : Request(GameId, PlayerId);

public record PlayerReadyResponse(IReadOnlyList<DomainEvent> Events) : CommandResponse(Events);

public class PlayerReadyUsecase(ICommandRepository repository, IEventBus<DomainEvent> eventBus)
    : CommandUsecase<PlayerReadyRequest, PlayerReadyResponse>(repository, eventBus)
{
    public override async Task ExecuteAsync(PlayerReadyRequest request, IPresenter<PlayerReadyResponse> presenter)
    {
        //查
        var game = Repository.FindGameById(request.GameId).ToDomain();

        //改
        game.PlayerReady(request.PlayerId);

        //存
        Repository.Save(game);

        //推
        await presenter.PresentAsync(new PlayerReadyResponse(game.DomainEvents));
    }
}