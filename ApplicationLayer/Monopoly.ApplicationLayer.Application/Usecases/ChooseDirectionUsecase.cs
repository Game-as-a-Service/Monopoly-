using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.DomainLayer.Common;

namespace Monopoly.ApplicationLayer.Application.Usecases;

public record ChooseDirectionRequest(string GameId, string PlayerId, string Direction)
    : GameRequest(GameId, PlayerId);

public record ChooseDirectionResponse(IReadOnlyList<DomainEvent> Events) : CommandResponse(Events);

public class ChooseDirectionUsecase(ICommandRepository repository, IEventBus<DomainEvent> eventBus)
    : Usecase<ChooseDirectionRequest, ChooseDirectionResponse>
{
    public override async Task ExecuteAsync(ChooseDirectionRequest request,
        IPresenter<ChooseDirectionResponse> presenter, CancellationToken cancellationToken = default)
    {
        //查
        var game = repository.FindGameById(request.GameId);
        //改
        game.PlayerSelectDirection(request.PlayerId, request.Direction);
        //存
        repository.Save(game);
        //推
        await presenter.PresentAsync(new ChooseDirectionResponse(game.DomainEvents), cancellationToken);
    }
}