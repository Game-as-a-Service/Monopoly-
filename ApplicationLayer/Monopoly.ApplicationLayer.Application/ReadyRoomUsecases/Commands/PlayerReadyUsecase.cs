using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.DomainLayer.Common;
using Monopoly.DomainLayer.ReadyRoom;

namespace Monopoly.ApplicationLayer.Application.ReadyRoomUsecases.Commands;

public record PlayerReadyRequest(string GameId, string PlayerId)
    : GameRequest(GameId, PlayerId);

public class PlayerReadyUsecase(IRepository<ReadyRoomAggregate> repository, IEventBus<DomainEvent> eventBus)
    : Usecase<PlayerReadyRequest>
{
    public override async Task ExecuteAsync(PlayerReadyRequest request, CancellationToken cancellationToken = default)
    {
        //查
        var readyRoom = await repository.FindByIdAsync(request.GameId);

        //改
        readyRoom.PlayerReady(request.PlayerId);

        //存
        await repository.SaveAsync(readyRoom);

        //推
        await eventBus.PublishAsync(readyRoom.DomainEvents, cancellationToken);
    }
}