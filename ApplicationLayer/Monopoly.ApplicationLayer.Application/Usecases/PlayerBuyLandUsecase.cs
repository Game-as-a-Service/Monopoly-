using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.DomainLayer.Common;

namespace Monopoly.ApplicationLayer.Application.Usecases;

public record PlayerBuyLandRequest(string GameId, string PlayerId, string LandID)
    : GameRequest(GameId, PlayerId);

public record PlayerBuyLandResponse(IReadOnlyList<DomainEvent> Events) : CommandResponse(Events);

public class PlayerBuyLandUsecase(ICommandRepository repository, IEventBus<DomainEvent> eventBus)
    : CommandUsecase<PlayerBuyLandRequest, PlayerBuyLandResponse>(repository, eventBus)
{
    public override async Task ExecuteAsync(PlayerBuyLandRequest request, IPresenter<PlayerBuyLandResponse> presenter,
        CancellationToken cancellationToken = default)
    {
        //查
        var game = Repository.FindGameById(request.GameId).ToDomain();

        //改
        game.BuyLand(request.PlayerId, request.LandID);

        //存
        Repository.Save(game);

        //推
        await presenter.PresentAsync(new PlayerBuyLandResponse(game.DomainEvents), cancellationToken);
    }
}