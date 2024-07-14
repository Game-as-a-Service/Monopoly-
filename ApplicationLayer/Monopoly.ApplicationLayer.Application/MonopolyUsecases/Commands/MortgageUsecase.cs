using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.DomainLayer.Common;
using Monopoly.DomainLayer.Domain;

namespace Monopoly.ApplicationLayer.Application.MonopolyUsecases.Commands;

public record MortgageRequest(string GameId, string PlayerId, string BlockId)
    : GameRequest(GameId, PlayerId);

public record MortgageResponse(IReadOnlyList<DomainEvent> Events) : CommandResponse(Events);

public class MortgageUsecase(IRepository<MonopolyAggregate> repository, IEventBus<DomainEvent> eventBus)
    : Usecase<MortgageRequest, MortgageResponse>
{
    public override async Task ExecuteAsync(MortgageRequest request, IPresenter<MortgageResponse> presenter,
        CancellationToken cancellationToken = default)
    {
        //查
        var game = await repository.FindByIdAsync(request.GameId);

        //改
        game.MortgageLandContract(request.PlayerId, request.BlockId);

        //存
        await repository.SaveAsync(game);

        //推
        await presenter.PresentAsync(new MortgageResponse(game.DomainEvents), cancellationToken);
    }
}