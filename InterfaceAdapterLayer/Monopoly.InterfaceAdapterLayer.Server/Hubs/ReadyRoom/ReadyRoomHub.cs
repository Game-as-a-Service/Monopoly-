using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Monopoly.ApplicationLayer.Application.Queries;
using Monopoly.ApplicationLayer.Application.Usecases.ReadyRoom;
using Monopoly.InterfaceAdapterLayer.Server.Presenters;
using SharedLibrary;
using SharedLibrary.ResponseArgs.ReadyRoom.Models;

namespace Monopoly.InterfaceAdapterLayer.Server.Hubs.ReadyRoom;

[Authorize]
public sealed class ReadyRoomHub : Hub<IReadyRoomResponses>
{
    private const string KeyOfPlayerId = "PlayerId";
    private const string KeyOfGameId = "GameId";
    private string GameId => Context.Items[KeyOfGameId] as string ?? "";
    private string PlayerId => Context.Items[KeyOfPlayerId] as string ?? "";

    public async Task StartGame(StartGameUsecase usecase)
    {
        await usecase.ExecuteAsync(
            new StartGameRequest(GameId, PlayerId),
            NullPresenter<StartGameResponse>.Instance);
    }

    public async Task PlayerReady(PlayerReadyUsecase usecase)
    {
        await usecase.ExecuteAsync(
            new PlayerReadyRequest(GameId, PlayerId),
            NullPresenter<PlayerReadyResponse>.Instance);
    }

    public async Task SelectLocation(LocationEnum location, SelectLocationUsecase usecase)
    {
        await usecase.ExecuteAsync(
            new SelectLocationRequest(GameId, PlayerId, location.AsDomainLocationEnum()),
            NullPresenter<SelectLocationResponse>.Instance);
    }

    public async Task SelectRole(string role, SelectRoleUsecase usecase)
    {
        await usecase.ExecuteAsync(
            new SelectRoleRequest(GameId, PlayerId, role),
            NullPresenter<SelectRoleResponse>.Instance);
    }

    public async Task<ReadyRoomInfos> GetReadyRoomInfos(GetReadyRoomInfosUsecase usecase)
    {
        var presenter = new ReadyRoomInfosValueReturningPresenter();
        await usecase.ExecuteAsync(
            new GetReadyRoomInfosRequest(GameId, PlayerId),
            presenter
        );
        return await presenter.GetResultAsync();
    }
    
    public async Task JoinRoom(PlayerJoinReadyRoomUsecase usecase)
    {
        await usecase.ExecuteAsync(
            new PlayerJoinReadyRoomRequest(GameId, PlayerId),
            NullPresenter<PlayerJoinReadyRoomResponse>.Instance);
    }

    public override async Task OnConnectedAsync()
    {
        var playerId = Context.UserIdentifier;
        var gameId = Context.GetHttpContext()!.Request.Query["gameId"][0];
        Context.Items[KeyOfPlayerId] = playerId;
        Context.Items[KeyOfGameId] = gameId;
        await base.OnConnectedAsync();
    }
}