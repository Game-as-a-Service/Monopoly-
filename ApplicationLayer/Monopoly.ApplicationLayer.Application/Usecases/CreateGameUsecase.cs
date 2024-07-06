using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.DomainLayer.Common;
using Monopoly.DomainLayer.Domain.Builders;
using Monopoly.DomainLayer.Domain.Maps;

namespace Monopoly.ApplicationLayer.Application.Usecases;

public record CreateGameRequest(string HostId, string[] PlayerIds) : GameRequest(null!, HostId);

public record CreateGameResponse(string GameId) : Response;

public class CreateGameUsecase(IRepository repository, IEventBus<DomainEvent> eventBus)
    : Usecase<CreateGameRequest, CreateGameResponse>
{
    public override async Task ExecuteAsync(CreateGameRequest gameRequest, IPresenter<CreateGameResponse> presenter,
        CancellationToken cancellationToken = default)
    {
        // 查
        // 改

        var builder = new MonopolyBuilder();
        foreach (var playerId in gameRequest.PlayerIds)
        {
            builder.WithPlayer(playerId);
        }
        builder.WithHost(gameRequest.HostId);
        builder.WithCurrentPlayer(gameRequest.PlayerIds.First());
        builder.WithMap(new SevenXSevenMap());

        // 存
        var id = repository.Save(builder.Build());

        // 推
        await presenter.PresentAsync(new CreateGameResponse(id), cancellationToken);
    }
}