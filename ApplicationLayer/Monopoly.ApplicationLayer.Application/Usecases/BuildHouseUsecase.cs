using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.DomainLayer.Common;

namespace Monopoly.ApplicationLayer.Application.Usecases;

public record BuildHouseRequest(string GameId, string PlayerId)
    : GameRequest(GameId, PlayerId);

public record BuildHouseResponse(IReadOnlyList<DomainEvent> Events) : CommandResponse(Events);

public class BuildHouseUsecase(ICommandRepository repository, IEventBus<DomainEvent> eventBus)
    : CommandUsecase<BuildHouseRequest, BuildHouseResponse>(repository, eventBus)
{
    public override async Task ExecuteAsync(BuildHouseRequest request, IPresenter<BuildHouseResponse> presenter,
        CancellationToken cancellationToken = default)
    {
        //查
        var game = Repository.FindGameById(request.GameId).ToDomain();

        //改
        game.BuildHouse(request.PlayerId);

        //存
        Repository.Save(game);

        //推
        await presenter.PresentAsync(new BuildHouseResponse(game.DomainEvents), cancellationToken);
    }
}