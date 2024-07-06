using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.DomainLayer.Common;

namespace Monopoly.ApplicationLayer.Application.Usecases;

public record PlayerBuyLandRequest(string GameId, string PlayerId, string LandID)
    : GameRequest(GameId, PlayerId);

public record PlayerBuyLandResponse(IReadOnlyList<DomainEvent> Events) : CommandResponse(Events);

public class PlayerBuyLandUsecase(ICommandRepository repository, IEventBus<DomainEvent> eventBus)
    : Usecase<PlayerBuyLandRequest, PlayerBuyLandResponse>
{
    public override async Task ExecuteAsync(PlayerBuyLandRequest request, IPresenter<PlayerBuyLandResponse> presenter,
        CancellationToken cancellationToken = default)
    {
        //查
        var game = repository.FindGameById(request.GameId);

        //改
        game.BuyLand(request.PlayerId, request.LandID);

        //存
        repository.Save(game);

        //推
        await presenter.PresentAsync(new PlayerBuyLandResponse(game.DomainEvents), cancellationToken);
    }
}