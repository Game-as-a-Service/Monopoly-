using Application.Common;
using Monopoly.DomainLayer.Common;

namespace Application.Usecases;

public record PayTollRequest(string GameId, string PlayerId)
    : GameRequest(GameId, PlayerId);

public record PayTollResponse(IReadOnlyList<DomainEvent> Events) : CommandResponse(Events);

public class PayTollUsecase(ICommandRepository repository, IEventBus<DomainEvent> eventBus)
    : CommandUsecase<PayTollRequest, PayTollResponse>(repository, eventBus)
{
    public override async Task ExecuteAsync(PayTollRequest request, IPresenter<PayTollResponse> presenter,
        CancellationToken cancellationToken = default)
    {
        //查
        var game = Repository.FindGameById(request.GameId).ToDomain();

        //改
        game.PayToll(request.PlayerId);

        //存
        Repository.Save(game);

        //推
        await presenter.PresentAsync(new PayTollResponse(game.DomainEvents), cancellationToken);
    }
}