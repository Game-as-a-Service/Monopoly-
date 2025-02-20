using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.DomainLayer.Common;
using Monopoly.DomainLayer.ReadyRoom;
using Monopoly.DomainLayer.ReadyRoom.Enums;

namespace Monopoly.ApplicationLayer.Application.ReadyRoomUsecases.Commands;

public record SelectLocationRequest(string GameId, string PlayerId, LocationEnum Location)
    : GameRequest(GameId, PlayerId);

public class SelectLocationUsecase(IRepository<ReadyRoomAggregate> repository, IEventBus<DomainEvent> eventBus)
    : Usecase<SelectLocationRequest>
{
    public override async Task ExecuteAsync(SelectLocationRequest request,
        CancellationToken cancellationToken = default)
    {
        //查
        var readyRoom = await repository.FindByIdAsync(request.GameId);

        //改
        readyRoom.SelectLocation(request.PlayerId, request.Location);

        //存
        await repository.SaveAsync(readyRoom);

        //推
        await eventBus.PublishAsync(readyRoom.DomainEvents, cancellationToken);
    }
}