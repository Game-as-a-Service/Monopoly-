using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.DomainLayer.Common;

namespace Monopoly.ApplicationLayer.Application.Usecases;

public record RedeemRequest(string GameId, string PlayerId, string BlockId)
    : GameRequest(GameId, PlayerId);

public record RedeemResponse(IReadOnlyList<DomainEvent> Events) : CommandResponse(Events);

public class RedeemUsecase(ICommandRepository repository, IEventBus<DomainEvent> eventBus)
    : Usecase<RedeemRequest, RedeemResponse>
{
    public override async Task ExecuteAsync(RedeemRequest request, IPresenter<RedeemResponse> presenter,
        CancellationToken cancellationToken = default)
    {
        //查
        var game = repository.FindGameById(request.GameId);

        //改
        game.RedeemLandContract(request.PlayerId, request.BlockId);

        //存
        repository.Save(game);

        //推
        await presenter.PresentAsync(new RedeemResponse(game.DomainEvents), cancellationToken);
    }
}