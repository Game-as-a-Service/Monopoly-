using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.DomainLayer.Common;

namespace Monopoly.ApplicationLayer.Application.Usecases;

public record EndAuctionRequest(string GameId, string PlayerId)
    : GameRequest(GameId, PlayerId);

public record EndAuctionResponse(IReadOnlyList<DomainEvent> Events) : CommandResponse(Events);

public class EndAuctionUsecase(ICommandRepository repository, IEventBus<DomainEvent> eventBus)
    : CommandUsecase<EndAuctionRequest, EndAuctionResponse>(repository, eventBus)
{
    public override async Task ExecuteAsync(EndAuctionRequest request, IPresenter<EndAuctionResponse> presenter,
        CancellationToken cancellationToken = default)
    {
        //查
        var game = Repository.FindGameById(request.GameId).ToDomain();

        //改
        game.EndAuction();

        //存
        Repository.Save(game);

        //推
        await presenter.PresentAsync(new EndAuctionResponse(game.DomainEvents), cancellationToken);
    }
}