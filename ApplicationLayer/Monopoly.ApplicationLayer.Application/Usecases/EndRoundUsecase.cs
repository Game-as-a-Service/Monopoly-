using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.DomainLayer.Common;

namespace Monopoly.ApplicationLayer.Application.Usecases;

public record EndRoundRequest(string GameId, string PlayerId)
    : GameRequest(GameId, PlayerId);

public record EndRoundResponse(IReadOnlyList<DomainEvent> Events) : CommandResponse(Events);

public class EndRoundUsecase(IRepository repository, IEventBus<DomainEvent> eventBus)
    : Usecase<EndRoundRequest, EndRoundResponse>
{
    public override async Task ExecuteAsync(EndRoundRequest request, IPresenter<EndRoundResponse> presenter,
        CancellationToken cancellationToken = default)
    {
        //查
        var game = repository.FindGameById(request.GameId);

        //改
        game.EndRound();

        //存
        repository.Save(game);

        //推
        await presenter.PresentAsync(new EndRoundResponse(game.DomainEvents), cancellationToken);
    }
}