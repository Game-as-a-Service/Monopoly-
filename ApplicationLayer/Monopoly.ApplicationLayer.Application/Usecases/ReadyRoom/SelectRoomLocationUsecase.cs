using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.DomainLayer.Common;
using Monopoly.DomainLayer.ReadyRoom.Enums;

namespace Monopoly.ApplicationLayer.Application.Usecases.ReadyRoom;

public record SelectLocationRequest(string GameId, string PlayerId, LocationEnum Location)
    : GameRequest(GameId, PlayerId);

public record SelectLocationResponse(IReadOnlyList<DomainEvent> Events) : CommandResponse(Events);

public class SelectLocationUsecase(IReadyRoomRepository repository, IEventBus<DomainEvent> eventBus)
    : Usecase<SelectLocationRequest, SelectLocationResponse>
{
    public override async Task ExecuteAsync(SelectLocationRequest request,
        IPresenter<SelectLocationResponse> presenter, CancellationToken cancellationToken = default)
    {
        //查
        var readyRoom = await repository.GetReadyRoomAsync(request.GameId);

        //改
        readyRoom.SelectLocation(request.PlayerId, request.Location);

        //存
        await repository.SaveReadyRoomAsync(readyRoom);

        //推
        await eventBus.PublishAsync(readyRoom.DomainEvents, cancellationToken);
    }
}