using Application.Common;
using Domain.Common;

namespace Application.Usecases.ReadyRoom;

public record GameStartRequest(string GameId, string PlayerId)
    : Request(GameId, PlayerId);

public record GameStartResponse(IReadOnlyList<DomainEvent> Events) : CommandResponse(Events);

public class GameStartUsecase(IReadyRoomRepository repository, IEventBus<DomainEvent> eventBus)
    : Usecase<GameStartRequest, GameStartResponse>
{
    public override async Task ExecuteAsync(GameStartRequest request, IPresenter<GameStartResponse> presenter)
    {
        //查
        var readyRoom = await repository.GetReadyRoomAsync(request.GameId);

        //改
        readyRoom.StartGame(request.PlayerId);

        //存
        await repository.SaveReadyRoomAsync(readyRoom);

        //推
        //await eventBus.PublishAsync(readyRoom.DomainEvents);
        //await presenter.PresentAsync(new GameStartResponse(readyRoom.DomainEvents));
    }
}