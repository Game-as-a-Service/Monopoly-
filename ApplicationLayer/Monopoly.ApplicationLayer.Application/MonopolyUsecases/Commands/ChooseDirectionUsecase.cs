using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.DomainLayer.Common;
using Monopoly.DomainLayer.Domain;

namespace Monopoly.ApplicationLayer.Application.MonopolyUsecases.Commands;

public record ChooseDirectionRequest(string GameId, string PlayerId, string Direction)
    : GameRequest(GameId, PlayerId);

public record ChooseDirectionResponse(IReadOnlyList<DomainEvent> Events) : CommandResponse(Events);

public class ChooseDirectionUsecase(IRepository<MonopolyAggregate> repository, IEventBus<DomainEvent> eventBus)
    : Usecase<ChooseDirectionRequest, ChooseDirectionResponse>
{
    public override async Task ExecuteAsync(ChooseDirectionRequest request,
        IPresenter<ChooseDirectionResponse> presenter, CancellationToken cancellationToken = default)
    {
        //查
        var game = repository.FindById(request.GameId);
        //改
        game.PlayerSelectDirection(request.PlayerId, request.Direction);
        //存
        repository.Save(game);
        //推
        await presenter.PresentAsync(new ChooseDirectionResponse(game.DomainEvents), cancellationToken);
    }
}