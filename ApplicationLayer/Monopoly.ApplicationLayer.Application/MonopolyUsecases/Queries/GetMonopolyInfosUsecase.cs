using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.ApplicationLayer.Application.ReadyRoomUsecases.Queries.Interfaces;
using Monopoly.DomainLayer.Domain;

namespace Monopoly.ApplicationLayer.Application.MonopolyUsecases.Queries;

public sealed record GetMonopolyInfosRequest(string GameId, string RequestPlayerId) : BaseRequest;

public sealed record MonopolyInfosResponse(
    string RequestPlayerId,
    MonopolyAggregate Monopoly,
    IEnumerable<IGetReadyRoomAllPlayerInfosInquiry.PlayerInfos> PlayerInfosInReadyRoom) : Response;

public sealed class GetMonopolyInfosUsecase(
    IGetReadyRoomAllPlayerInfosInquiry getReadyRoomAllPlayerInfosInquiry,
    IRepository<MonopolyAggregate> repository)
    : Usecase<GetMonopolyInfosRequest, MonopolyInfosResponse>
{
    public override async Task ExecuteAsync(GetMonopolyInfosRequest infosRequest,
        IPresenter<MonopolyInfosResponse> presenter, CancellationToken cancellationToken = default)
    {
        var monopoly = await repository.FindByIdAsync(infosRequest.GameId);
        var playerInfosInReadyRoom =
            await getReadyRoomAllPlayerInfosInquiry.GetReadyRoomAllPlayerInfosAsync(infosRequest.GameId);
        var monopolyInfos = new MonopolyInfosResponse(infosRequest.RequestPlayerId, monopoly, playerInfosInReadyRoom);

        await presenter.PresentAsync(monopolyInfos, cancellationToken);
    }
}