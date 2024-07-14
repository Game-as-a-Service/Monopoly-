using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.DomainLayer.Common;
using Monopoly.DomainLayer.ReadyRoom;

namespace Monopoly.ApplicationLayer.Application.ReadyRoomUsecases.Commands;

public record SelectRoleRequest(string GameId, string PlayerId, string Role)
    : GameRequest(GameId, PlayerId);

public class SelectRoleUsecase(IRepository<ReadyRoomAggregate> repository, IEventBus<DomainEvent> eventBus)
    : Usecase<SelectRoleRequest>
{
    public override async Task ExecuteAsync(SelectRoleRequest request, CancellationToken cancellationToken = default)
    {
        //查
        var readyRoom = await repository.FindByIdAsync(request.GameId);

        //改
        readyRoom.SelectRole(request.PlayerId, request.Role);

        //存
        await repository.SaveAsync(readyRoom);

        //推
        await eventBus.PublishAsync(readyRoom.DomainEvents, cancellationToken);
    }
}