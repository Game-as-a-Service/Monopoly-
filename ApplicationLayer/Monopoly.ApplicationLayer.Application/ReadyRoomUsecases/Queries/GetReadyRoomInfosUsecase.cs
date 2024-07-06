using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.ApplicationLayer.Application.ReadyRoomUsecases.Commands;
using Monopoly.DomainLayer.ReadyRoom;

namespace Monopoly.ApplicationLayer.Application.ReadyRoomUsecases.Queries;

public record GetReadyRoomInfosRequest(string GameId, string PlayerId)
    : GameRequest(GameId, PlayerId);
public record GetReadyRoomInfosResponse(string RequestPlayerId, ReadyRoomAggregate ReadyRoom) : Response;

public class GetReadyRoomInfosUsecase(IReadyRoomRepository repository)
    : Usecase<GetReadyRoomInfosRequest, GetReadyRoomInfosResponse>
{
    public override async Task ExecuteAsync(GetReadyRoomInfosRequest request,
        IPresenter<GetReadyRoomInfosResponse> presenter, CancellationToken cancellationToken = default)
    {
        var readyRoom = await repository.GetReadyRoomAsync(request.GameId);
        await presenter.PresentAsync(new GetReadyRoomInfosResponse(request.PlayerId, readyRoom), cancellationToken);
    }
}
