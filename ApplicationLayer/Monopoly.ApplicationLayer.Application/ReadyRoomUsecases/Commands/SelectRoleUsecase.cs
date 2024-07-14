using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.DomainLayer.Common;

namespace Monopoly.ApplicationLayer.Application.ReadyRoomUsecases.Commands;

public record SelectRoleRequest(string GameId, string PlayerId, string Role)
    : GameRequest(GameId, PlayerId);

public record SelectRoleResponse(IReadOnlyList<DomainEvent> Events) : CommandResponse(Events);

public class SelectRoleUsecase(IReadyRoomRepository repository, IEventBus<DomainEvent> eventBus)
    : Usecase<SelectRoleRequest, SelectRoleResponse>
{
    public override async Task ExecuteAsync(SelectRoleRequest request, IPresenter<SelectRoleResponse> presenter,
        CancellationToken cancellationToken = default)
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