using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.DomainLayer.Common;
using Monopoly.DomainLayer.Domain;

namespace Monopoly.ApplicationLayer.Application.MonopolyUsecases.Commands;

public record SettlementRequest(string GameId, string PlayerId)
    : GameRequest(GameId, PlayerId);

public record SettlementResponse(IReadOnlyList<DomainEvent> Events) : CommandResponse(Events);

public class SettlementUsecase(IRepository<MonopolyAggregate> repository, IEventBus<DomainEvent> eventBus)
    : Usecase<SettlementRequest, SettlementResponse>
{
    public override async Task ExecuteAsync(SettlementRequest request, IPresenter<SettlementResponse> presenter,
        CancellationToken cancellationToken = default)
    {
        //查
        var game = await repository.FindByIdAsync(request.GameId);

        //改
        game.Settlement();

        //存
        await repository.SaveAsync(game);

        //推
        await presenter.PresentAsync(new SettlementResponse(game.DomainEvents), cancellationToken);
    }
}