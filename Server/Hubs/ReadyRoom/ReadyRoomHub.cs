using System.Security.Claims;
using Application.Usecases.ReadyRoom;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Server.Presenters;
using SharedLibrary;

namespace Server.Hubs.ReadyRoom;

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
            NullPresenter<StartGameResponse>.Instance
        );
    }
    
    public async Task PlayerReady(PlayerReadyUsecase usecase)
    {
        await usecase.ExecuteAsync(
            new PlayerReadyRequest(GameId, PlayerId),
            NullPresenter<PlayerReadyResponse>.Instance
        );
    }
    
    public async Task SelectLocation(int location, SelectLocationUsecase usecase)
    {
        await usecase.ExecuteAsync(
            new SelectLocationRequest(GameId, PlayerId, location),
            NullPresenter<SelectLocationResponse>.Instance
        );
    }
    
    public async Task SelectRole(string role, SelectRoleUsecase usecase)
    {
        await usecase.ExecuteAsync(
            new SelectRoleRequest(GameId, PlayerId, role),
            NullPresenter<SelectRoleResponse>.Instance
        );
    }
    
    public override async Task OnConnectedAsync()
    {
        var playerId = Context.User!.FindFirst(x => x.Type == ClaimTypes.Sid)!.Value;
        var gameId = Context.GetHttpContext()!.Request.Query["gameId"][0];
        Context.Items[KeyOfPlayerId] = playerId;
        Context.Items[KeyOfGameId] = gameId;
        await base.OnConnectedAsync();
    }
}