using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.DomainLayer.ReadyRoom;

namespace Monopoly.ApplicationLayer.Application.ReadyRoomUsecases.Commands;

public record CreateReadyRoomRequest(string PlayerId) : BaseRequest;

public record CreateReadyRoomResponse(string RoomId) : Response;

public sealed class CreateReadyRoomUsecase(IReadyRoomRepository repository) : Usecase<CreateReadyRoomRequest, CreateReadyRoomResponse>
{
    public override async Task ExecuteAsync(CreateReadyRoomRequest request, IPresenter<CreateReadyRoomResponse> presenter, CancellationToken cancellationToken = default)
    {
        var readyRoom = ReadyRoomAggregate.Builder()
            .WithHost(request.PlayerId)
            .Build();
        
        await repository.SaveAsync(readyRoom);
        
        await presenter.PresentAsync(new CreateReadyRoomResponse(readyRoom.Id), cancellationToken);
    }
}
