using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.DomainLayer.Common;

namespace Monopoly.ApplicationLayer.Application.Usecases.ReadyRoom;

public record PlayerJoinReadyRoomRequest(string RoomId, string PlayerId) : BaseRequest;

public record PlayerJoinReadyRoomResponse : Response;

public class PlayerJoinReadyRoomUsecase(IReadyRoomRepository repository, IEventBus<DomainEvent> eventBus)
    : Usecase<PlayerJoinReadyRoomRequest, PlayerJoinReadyRoomResponse>
{
    public override async Task ExecuteAsync(PlayerJoinReadyRoomRequest request,
        IPresenter<PlayerJoinReadyRoomResponse> presenter,
        CancellationToken cancellationToken = default)
    {
        // 查
        var readyRoom = await repository.GetReadyRoomAsync(request.RoomId);
        
        // 改
        readyRoom.PlayerJoin(request.PlayerId);
        
        // 存
        await repository.SaveReadyRoomAsync(readyRoom);
        
        // 推
        await eventBus.PublishAsync(readyRoom.DomainEvents, cancellationToken);
    }
}