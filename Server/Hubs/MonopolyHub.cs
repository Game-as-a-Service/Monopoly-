using Application.Common;
using Application.Query;
using Application.Usecases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Server.Presenters;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;
using SharedLibrary.ResponseArgs.ReadyRoom;
using System.Security.Claims;

namespace Server.Hubs;

[Authorize]
public class MonopolyHub(ICommandRepository repository) : Hub<IMonopolyResponses>
{
    private const string KeyOfPlayerId = "PlayerId";
    private const string KeyOfGameId = "GameId";
    private string GameId => Context.Items[KeyOfGameId] as string ?? "";
    private string PlayerId => Context.Items[KeyOfPlayerId] as string ?? "";

    public async Task PlayerRollDice(PlayerRollDiceUsecase usecase, SignalrDefaultPresenter<PlayerRollDiceResponse> presenter)
    {
        await usecase.ExecuteAsync(
            new PlayerRollDiceRequest(GameId, PlayerId),
            presenter
            );
    }

    public async Task PlayerChooseDirection(string direction, ChooseDirectionUsecase usecase, SignalrDefaultPresenter<ChooseDirectionResponse> presenter)
    {
        await usecase.ExecuteAsync(
            new ChooseDirectionRequest(GameId, PlayerId, direction),
            presenter
            );
    }

    public async Task PlayerBuyLand(string blockId, PlayerBuyLandUsecase usecase, SignalrDefaultPresenter<PlayerBuyLandResponse> presenter)
    {
        await usecase.ExecuteAsync(
            new PlayerBuyLandRequest(GameId, PlayerId, blockId),
            presenter
            );
    }

    public async Task PlayerPayToll(PayTollUsecase usecase, SignalrDefaultPresenter<PayTollResponse> presenter)
    {
        await usecase.ExecuteAsync(
            new PayTollRequest(GameId, PlayerId),
            presenter
            );
    }

    public async Task PlayerSelectLocation(int locationId, SelectLocationUsecase usecase, SignalrDefaultPresenter<SelectLocationResponse> presenter)
    {
        await usecase.ExecuteAsync(
            new SelectLocationRequest(GameId, PlayerId, locationId),
            presenter
            );
    }

    public async Task PlayerBuildHouse(BuildHouseUsecase usecase, SignalrDefaultPresenter<BuildHouseResponse> presenter)
    {
        await usecase.ExecuteAsync(
            new BuildHouseRequest(GameId, PlayerId),
            presenter
            );
    }

    public async Task PlayerMortgage(string blockId, MortgageUsecase usecase, SignalrDefaultPresenter<MortgageResponse> presenter)
    {
        await usecase.ExecuteAsync(
            new MortgageRequest(GameId, PlayerId, blockId),
            presenter
            );
    }

    public async Task PlayerRedeem(string blockId, RedeemUsecase usecase, SignalrDefaultPresenter<RedeemResponse> presenter)
    {
        await usecase.ExecuteAsync(
            new RedeemRequest(GameId, PlayerId, blockId),
            presenter
            );
    }

    public async Task PlayerBid(decimal bidPrice, BidUsecase usecase, SignalrDefaultPresenter<BidResponse> presenter)
    {
        await usecase.ExecuteAsync(
            new BidRequest(GameId, PlayerId, bidPrice),
            presenter
            );
    }

    public async Task EndAuction(EndAuctionUsecase usecase, SignalrDefaultPresenter<EndAuctionResponse> presenter)
    {
        await usecase.ExecuteAsync(
            new EndAuctionRequest(GameId, PlayerId),
            presenter
            );
    }

    public async Task EndRound(EndRoundUsecase usecase, SignalrDefaultPresenter<EndRoundResponse> presenter)
    {
        await usecase.ExecuteAsync(
            new EndRoundRequest(GameId, PlayerId),
            presenter
            );
    }

    public async Task Settlement(SettlementUsecase usecase, SignalrDefaultPresenter<SettlementResponse> presenter)
    {
        await usecase.ExecuteAsync(
            new SettlementRequest(GameId, PlayerId),
            presenter
            );
    }

    public async Task PlayerSelectRole(string roleId, SelectRoleUsecase usecase, SignalrDefaultPresenter<SelectRoleResponse> presenter)
    {
        await usecase.ExecuteAsync(
            new SelectRoleRequest(GameId, PlayerId, roleId),
            presenter
            );
    }

    public async Task PlayerReady(PlayerReadyUsecase usecase, SignalrDefaultPresenter<PlayerReadyResponse> presenter)
    {
        await usecase.ExecuteAsync(
            new PlayerReadyRequest(GameId, PlayerId),
            presenter
            );
    }

    public async Task GameStart(GameStartUsecase usecase, SignalrDefaultPresenter<GameStartResponse> presenter)
    {
        await usecase.ExecuteAsync(
            new GameStartRequest(GameId, PlayerId),
            presenter
            );
    }

    public async Task GetReadyInfo(GetReadyInfoUsecase usecase)
    {
        var presenter = new DefaultPresenter<GetReadyInfoResponse>();
        await usecase.ExecuteAsync(new GetReadyInfoRequest(GameId, PlayerId), presenter);
        await Clients.Caller.GetReadyInfoEvent(new GetReadyInfoEventArgs
        {
            Players = presenter.Value.Info.Players.Select(x =>
            {
                var isRole = Enum.TryParse<GetReadyInfoEventArgs.RoleEnum>(x.RoleId, true, out var roleEnum);
                if (isRole is false)
                {
                    roleEnum = GetReadyInfoEventArgs.RoleEnum.None;
                }
                return new GetReadyInfoEventArgs.Player
                {
                    Id = x.PlayerId,
                    Name = x.PlayerId,
                    IsReady = x.IsReady,
                    Role = roleEnum,
                    Color = (GetReadyInfoEventArgs.ColorEnum?)x.LocationId ?? GetReadyInfoEventArgs.ColorEnum.None
                };
            }).ToList(),
            HostId = presenter.Value.Info.HostId,
        });
    }

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext()!;
        var gameIdStringValues = httpContext.Request.Query["gameId"];
        if (gameIdStringValues.Count is 0)
        {
            throw new GameNotFoundException($"Not pass game id");
        }
        Context.Items[KeyOfGameId] = gameIdStringValues.ToString();
        Context.Items[KeyOfPlayerId] = Context.User!.FindFirst(x => x.Type == ClaimTypes.Sid)!.Value;
        if (repository.IsExist(GameId) is false)
        {
            throw new GameNotFoundException($"Can not find the game that id is {GameId}");
        }
        await Groups.AddToGroupAsync(Context.ConnectionId, GameId);
        await Clients.Caller.WelcomeEvent(new WelcomeEventArgs { PlayerId = PlayerId });
        await Clients.Group(GameId).PlayerJoinGameEvent(new PlayerJoinGameEventArgs { PlayerId = PlayerId });
    }

    private class GameNotFoundException(string message) : Exception(message);
}