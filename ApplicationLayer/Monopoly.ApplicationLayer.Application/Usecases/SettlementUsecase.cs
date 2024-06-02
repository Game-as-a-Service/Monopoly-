using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.DomainLayer.Common;

namespace Monopoly.ApplicationLayer.Application.Usecases;

public record SettlementRequest(string GameId, string PlayerId)
    : GameRequest(GameId, PlayerId);

public record SettlementResponse(IReadOnlyList<DomainEvent> Events) : CommandResponse(Events);

public class SettlementUsecase(ICommandRepository repository, IEventBus<DomainEvent> eventBus)
    : CommandUsecase<SettlementRequest, SettlementResponse>(repository, eventBus)
{
    public override async Task ExecuteAsync(SettlementRequest request, IPresenter<SettlementResponse> presenter,
        CancellationToken cancellationToken = default)
    {
        //查
        var game = Repository.FindGameById(request.GameId).ToDomain();

        //改
        game.Settlement();

        //存
        Repository.Save(game);

        //推
        await presenter.PresentAsync(new SettlementResponse(game.DomainEvents), cancellationToken);
    }
}