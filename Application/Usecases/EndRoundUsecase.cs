using Application.Common;
using Monopoly.DomainLayer.Common;

namespace Application.Usecases;

public record EndRoundRequest(string GameId, string PlayerId)
    : GameRequest(GameId, PlayerId);

public record EndRoundResponse(IReadOnlyList<DomainEvent> Events) : CommandResponse(Events);

public class EndRoundUsecase(ICommandRepository repository, IEventBus<DomainEvent> eventBus)
    : CommandUsecase<EndRoundRequest, EndRoundResponse>(repository, eventBus)
{
    public override async Task ExecuteAsync(EndRoundRequest request, IPresenter<EndRoundResponse> presenter,
        CancellationToken cancellationToken = default)
    {
        //查
        var game = Repository.FindGameById(request.GameId).ToDomain();

        //改
        game.EndRound();

        //存
        Repository.Save(game);

        //推
        await presenter.PresentAsync(new EndRoundResponse(game.DomainEvents), cancellationToken);
    }
}