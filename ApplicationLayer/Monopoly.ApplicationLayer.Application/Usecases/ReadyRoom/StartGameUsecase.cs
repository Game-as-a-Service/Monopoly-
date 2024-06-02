using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.DomainLayer.Common;

namespace Monopoly.ApplicationLayer.Application.Usecases.ReadyRoom;

public record StartGameRequest(string GameId, string PlayerId)
    : GameRequest(GameId, PlayerId);

public record StartGameResponse(IReadOnlyList<DomainEvent> Events) : CommandResponse(Events);

public class StartGameUsecase(IReadyRoomRepository repository, IEventBus<DomainEvent> eventBus)
    : Usecase<StartGameRequest, StartGameResponse>
{
    public override async Task ExecuteAsync(StartGameRequest gameRequest, IPresenter<StartGameResponse> presenter,
        CancellationToken cancellationToken = default)
    {
        //查
        var readyRoom = await repository.GetReadyRoomAsync(gameRequest.GameId);

        //改
        readyRoom.StartGame(gameRequest.PlayerId);

        //存
        await repository.SaveReadyRoomAsync(readyRoom);

        //推
        await eventBus.PublishAsync(readyRoom.DomainEvents, cancellationToken);
        //await presenter.PresentAsync(new GameStartResponse(readyRoom.DomainEvents));
    }
}