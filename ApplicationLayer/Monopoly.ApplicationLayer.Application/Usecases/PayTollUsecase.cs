using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.DomainLayer.Common;
using Monopoly.DomainLayer.Domain;

namespace Monopoly.ApplicationLayer.Application.Usecases;

public record PayTollRequest(string GameId, string PlayerId)
    : GameRequest(GameId, PlayerId);

public record PayTollResponse(IReadOnlyList<DomainEvent> Events) : CommandResponse(Events);

public class PayTollUsecase(IRepository<MonopolyAggregate> repository, IEventBus<DomainEvent> eventBus)
    : Usecase<PayTollRequest, PayTollResponse>
{
    public override async Task ExecuteAsync(PayTollRequest request, IPresenter<PayTollResponse> presenter,
        CancellationToken cancellationToken = default)
    {
        //查
        var game = repository.FindById(request.GameId);

        //改
        game.PayToll(request.PlayerId);

        //存
        repository.Save(game);

        //推
        await presenter.PresentAsync(new PayTollResponse(game.DomainEvents), cancellationToken);
    }
}