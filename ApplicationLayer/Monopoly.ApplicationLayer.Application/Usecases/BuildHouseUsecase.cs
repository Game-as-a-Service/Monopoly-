using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.DomainLayer.Common;

namespace Monopoly.ApplicationLayer.Application.Usecases;

public record BuildHouseRequest(string GameId, string PlayerId)
    : GameRequest(GameId, PlayerId);

public record BuildHouseResponse(IReadOnlyList<DomainEvent> Events) : CommandResponse(Events);

public class BuildHouseUsecase(IRepository repository, IEventBus<DomainEvent> eventBus)
    : Usecase<BuildHouseRequest, BuildHouseResponse>
{
    public override async Task ExecuteAsync(BuildHouseRequest request, IPresenter<BuildHouseResponse> presenter,
        CancellationToken cancellationToken = default)
    {
        //查
        var game = repository.FindGameById(request.GameId);

        //改
        game.BuildHouse(request.PlayerId);

        //存
        repository.Save(game);

        //推
        await presenter.PresentAsync(new BuildHouseResponse(game.DomainEvents), cancellationToken);
    }
}