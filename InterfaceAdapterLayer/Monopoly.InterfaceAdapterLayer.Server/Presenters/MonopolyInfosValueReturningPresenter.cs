using Monopoly.ApplicationLayer.Application.MonopolyUsecases.Queries;
using ResponseMonopolyInfos = SharedLibrary.ResponseArgs.Monopoly.Models.MonopolyInfos;

namespace Monopoly.InterfaceAdapterLayer.Server.Presenters;

public sealed class MonopolyInfosValueReturningPresenter : ValueReturningPresenter<ResponseMonopolyInfos, MonopolyInfosResponse>
{
    protected override Task<ResponseMonopolyInfos> TransformAsync(MonopolyInfosResponse response,
        CancellationToken cancellationToken)
    {
        var monopoly = response.Monopoly;
        var monopolyInfos = new ResponseMonopolyInfos(
            response.RequestPlayerId,
            monopoly.HostId,
            response.Monopoly.CurrentPlayerState.PlayerId,
            [
                ..response.PlayerInfosInReadyRoom.Select(player => new ResponseMonopolyInfos.PlayerInfos(
                    player.Id,
                    player.Name,
                    player.Color,
                    player.Role
                ))
            ]
        );
        return Task.FromResult(monopolyInfos);
    }
}