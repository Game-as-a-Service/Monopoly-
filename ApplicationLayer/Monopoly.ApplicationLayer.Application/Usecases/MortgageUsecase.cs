using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.DomainLayer.Common;

namespace Monopoly.ApplicationLayer.Application.Usecases;

public record MortgageRequest(string GameId, string PlayerId, string BlockId)
    : GameRequest(GameId, PlayerId);

public record MortgageResponse(IReadOnlyList<DomainEvent> Events) : CommandResponse(Events);

public class MortgageUsecase(IRepository repository, IEventBus<DomainEvent> eventBus)
    : Usecase<MortgageRequest, MortgageResponse>
{
    public override async Task ExecuteAsync(MortgageRequest request, IPresenter<MortgageResponse> presenter,
        CancellationToken cancellationToken = default)
    {
        //查
        var game = repository.FindGameById(request.GameId);

        //改
        game.MortgageLandContract(request.PlayerId, request.BlockId);

        //存
        repository.Save(game);

        //推
        await presenter.PresentAsync(new MortgageResponse(game.DomainEvents), cancellationToken);
    }
}