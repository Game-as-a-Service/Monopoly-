using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.DomainLayer.Common;
using Monopoly.DomainLayer.Domain;
using Monopoly.DomainLayer.Domain.Builders;
using Monopoly.DomainLayer.Domain.Maps;

namespace Monopoly.ApplicationLayer.Application.ReadyRoomUsecases.Commands;

public record StartGameRequest(string GameId, string PlayerId)
    : GameRequest(GameId, PlayerId);

public record StartGameResponse(IReadOnlyList<DomainEvent> Events) : CommandResponse(Events);

public class StartGameUsecase(IReadyRoomRepository readyRoomRepository, IRepository<MonopolyAggregate> gameRepository, IEventBus<DomainEvent> eventBus)
    : Usecase<StartGameRequest, StartGameResponse>
{
    public override async Task ExecuteAsync(StartGameRequest gameRequest, IPresenter<StartGameResponse> presenter,
        CancellationToken cancellationToken = default)
    {
        //查
        var readyRoom = await readyRoomRepository.GetReadyRoomAsync(gameRequest.GameId);

        //改
        readyRoom.StartGame(gameRequest.PlayerId);

        var builder = new MonopolyBuilder();
        foreach (var player in readyRoom.Players)
        {
            builder.WithPlayer(player.Id);
        }
        builder.WithId(readyRoom.GameId)
            .WithHost(readyRoom.HostId)
            .WithCurrentPlayer(readyRoom.Players[0].Id)
            .WithMap(new SevenXSevenMap());
        var game = builder.Build();
        
        //存
        await readyRoomRepository.SaveReadyRoomAsync(readyRoom);
        gameRepository.Save(game);

        //推
        await eventBus.PublishAsync(readyRoom.DomainEvents, cancellationToken);
        //await presenter.PresentAsync(new GameStartResponse(readyRoom.DomainEvents));
    }
}