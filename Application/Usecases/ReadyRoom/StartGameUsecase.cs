using Application.Common;
using Monopoly.DomainLayer.Common;

namespace Application.Usecases.ReadyRoom;

public record StartGameRequest(string GameId, string PlayerId)
    : Request(GameId, PlayerId);

public record StartGameResponse(IReadOnlyList<DomainEvent> Events) : CommandResponse(Events);

public class StartGameUsecase(IReadyRoomRepository repository, IEventBus<DomainEvent> eventBus)
    : Usecase<StartGameRequest, StartGameResponse>
{
    public override async Task ExecuteAsync(StartGameRequest request, IPresenter<StartGameResponse> presenter)
    {
        //查
        var readyRoom = await repository.GetReadyRoomAsync(request.GameId);

        //改
        readyRoom.StartGame(request.PlayerId);

        //存
        await repository.SaveReadyRoomAsync(readyRoom);

        //推
        await eventBus.PublishAsync(readyRoom.DomainEvents);
        //await presenter.PresentAsync(new GameStartResponse(readyRoom.DomainEvents));
    }
}