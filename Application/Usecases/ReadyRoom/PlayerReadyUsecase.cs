using Application.Common;
using Monopoly.DomainLayer.Common;

namespace Application.Usecases.ReadyRoom;

public record PlayerReadyRequest(string GameId, string PlayerId)
    : Request(GameId, PlayerId);

public record PlayerReadyResponse(IReadOnlyList<DomainEvent> Events) : CommandResponse(Events);

public class PlayerReadyUsecase(IReadyRoomRepository repository, IEventBus<DomainEvent> eventBus)
    : Usecase<PlayerReadyRequest, PlayerReadyResponse>
{
    public override async Task ExecuteAsync(PlayerReadyRequest request, IPresenter<PlayerReadyResponse> presenter,
        CancellationToken cancellationToken = default)
    {
        //查
        var readyRoom = await repository.GetReadyRoomAsync(request.GameId);

        //改
        readyRoom.PlayerReady(request.PlayerId);

        //存
        await repository.SaveReadyRoomAsync(readyRoom);

        //推
        await eventBus.PublishAsync(readyRoom.DomainEvents, cancellationToken);
    }
}