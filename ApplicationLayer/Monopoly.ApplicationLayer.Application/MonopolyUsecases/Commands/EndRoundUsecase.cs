using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.DomainLayer.Common;
using Monopoly.DomainLayer.Domain;

namespace Monopoly.ApplicationLayer.Application.MonopolyUsecases.Commands;

public record EndRoundRequest(string GameId, string PlayerId)
    : GameRequest(GameId, PlayerId);

public record EndRoundResponse(IReadOnlyList<DomainEvent> Events) : CommandResponse(Events);

public class EndRoundUsecase(IRepository<MonopolyAggregate> repository, IEventBus<DomainEvent> eventBus)
    : Usecase<EndRoundRequest, EndRoundResponse>
{
    public override async Task ExecuteAsync(EndRoundRequest request, IPresenter<EndRoundResponse> presenter,
        CancellationToken cancellationToken = default)
    {
        //查
        var game = repository.FindById(request.GameId);

        //改
        game.EndRound();

        //存
        repository.Save(game);

        //推
        await presenter.PresentAsync(new EndRoundResponse(game.DomainEvents), cancellationToken);
    }
}