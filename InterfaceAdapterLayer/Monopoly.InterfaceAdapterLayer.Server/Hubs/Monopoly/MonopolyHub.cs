using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Monopoly.ApplicationLayer.Application.MonopolyUsecases.Commands;
using Monopoly.ApplicationLayer.Application.MonopolyUsecases.Queries;
using Monopoly.InterfaceAdapterLayer.Server.Presenters;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly.Models;

namespace Monopoly.InterfaceAdapterLayer.Server.Hubs.Monopoly;

[Authorize]
public class MonopolyHub(CheckGameExistenceQuery checkGameExistenceQuery) : Hub<IMonopolyResponses>
{
    private const string KeyOfPlayerId = "PlayerId";
    private const string KeyOfGameId = "GameId";
    private string GameId => Context.Items[KeyOfGameId] as string ?? "";
    private string PlayerId => Context.Items[KeyOfPlayerId] as string ?? "";

    public async Task PlayerRollDice(PlayerRollDiceUsecase usecase)
    {
        await usecase.ExecuteAsync(
            new PlayerRollDiceRequest(GameId, PlayerId));
    }

    public async Task PlayerChooseDirection(string direction, ChooseDirectionUsecase usecase)
    {
        await usecase.ExecuteAsync(
            new ChooseDirectionRequest(GameId, PlayerId, direction)
        );
    }

    public async Task PlayerBuyLand(string blockId, PlayerBuyLandUsecase usecase)
    {
        await usecase.ExecuteAsync(
            new PlayerBuyLandRequest(GameId, PlayerId, blockId)
        );
    }

    public async Task PlayerPayToll(PayTollUsecase usecase)
    {
        await usecase.ExecuteAsync(
            new PayTollRequest(GameId, PlayerId)
        );
    }

    public async Task PlayerBuildHouse(BuildHouseUsecase usecase)
    {
        await usecase.ExecuteAsync(
            new BuildHouseRequest(GameId, PlayerId));
    }

    public async Task PlayerMortgage(string blockId, MortgageUsecase usecase)
    {
        await usecase.ExecuteAsync(
            new MortgageRequest(GameId, PlayerId, blockId)
        );
    }

    public async Task PlayerRedeem(string blockId, RedeemUsecase usecase)
    {
        await usecase.ExecuteAsync(
            new RedeemRequest(GameId, PlayerId, blockId)
        );
    }

    public async Task PlayerBid(decimal bidPrice, BidUsecase usecase)
    {
        await usecase.ExecuteAsync(
            new BidRequest(GameId, PlayerId, bidPrice)
        );
    }

    public async Task EndAuction(EndAuctionUsecase usecase)
    {
        await usecase.ExecuteAsync(
            new EndAuctionRequest(GameId, PlayerId)
        );
    }

    public async Task EndRound(EndRoundUsecase usecase)
    {
        await usecase.ExecuteAsync(
            new EndRoundRequest(GameId, PlayerId)
        );
    }

    public async Task Settlement(SettlementUsecase usecase)
    {
        await usecase.ExecuteAsync(
            new SettlementRequest(GameId, PlayerId)
        );
    }
    
    public async Task<MonopolyInfos> GetMonopolyInfos(GetMonopolyInfosUsecase usecase)
    {
        var presenter = new MonopolyInfosValueReturningPresenter();
        await usecase.ExecuteAsync(
            new GetMonopolyInfosRequest(GameId, PlayerId),
            presenter
        );
        return await presenter.GetResultAsync();
    }

    public override async Task OnConnectedAsync()
    {
        ValidateAndSetGameIdAndPlayerId();

        await EnsureGameExists();

        await Groups.AddToGroupAsync(Context.ConnectionId, GameId);
        // 暫時不需要，之後考慮有沒有必要
        // await Clients.Caller.WelcomeEvent(new WelcomeEventArgs { PlayerId = PlayerId });
        // await Clients.Group(GameId).PlayerJoinGameEvent(new PlayerJoinGameEventArgs { PlayerId = PlayerId });
    }

    private async Task EnsureGameExists()
    {
        var presenter = new DefaultPresenter<CheckGameExistenceQuery.Response>();
        var checkGameExistenceRequest = new CheckGameExistenceQuery.Request(GameId);
        await checkGameExistenceQuery.ExecuteAsync(checkGameExistenceRequest, presenter);
        if (presenter.Value.IsExist is false)
        {
            throw new GameNotFoundException($"Can not find the game that id is {GameId}");
        }
    }

    private void ValidateAndSetGameIdAndPlayerId()
    {
        var httpContext = Context.GetHttpContext()!;
        var gameIdStringValues = httpContext.Request.Query["gameId"];
        if (gameIdStringValues.Count is 0)
        {
            throw new GameNotFoundException("Not pass game id");
        }

        Context.Items[KeyOfGameId] = gameIdStringValues.ToString();
        Context.Items[KeyOfPlayerId] = Context.UserIdentifier;
    }

    private class GameNotFoundException(string message) : Exception(message);
}