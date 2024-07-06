using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.DomainLayer.Common;

namespace Monopoly.ApplicationLayer.Application.Usecases;

public record BidRequest(string GameId, string PlayerId, decimal BidPrice)
    : GameRequest(GameId, PlayerId);

public record BidResponse(IReadOnlyList<DomainEvent> Events) : CommandResponse(Events);

public class BidUsecase(IRepository repository, IEventBus<DomainEvent> eventBus)
    : Usecase<BidRequest, BidResponse>
{
    public override async Task ExecuteAsync(BidRequest request, IPresenter<BidResponse> presenter,
        CancellationToken cancellationToken = default)
    {
        //查
        var game = repository.FindGameById(request.GameId);

        //改
        game.PlayerBid(request.PlayerId, request.BidPrice);

        //存
        repository.Save(game);

        //推
        await presenter.PresentAsync(new BidResponse(game.DomainEvents), cancellationToken);
    }
}