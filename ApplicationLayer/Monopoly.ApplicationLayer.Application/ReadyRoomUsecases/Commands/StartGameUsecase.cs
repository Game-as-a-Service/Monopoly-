using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.DomainLayer.Common;
using Monopoly.DomainLayer.Domain;
using Monopoly.DomainLayer.Domain.Builders;
using Monopoly.DomainLayer.Domain.Maps;
using Monopoly.DomainLayer.ReadyRoom;

namespace Monopoly.ApplicationLayer.Application.ReadyRoomUsecases.Commands;

public record StartGameRequest(string GameId, string PlayerId)
    : GameRequest(GameId, PlayerId);

public class StartGameUsecase(
    IRepository<ReadyRoomAggregate> readyRoomRepository,
    IRepository<MonopolyAggregate> gameRepository,
    IEventBus<DomainEvent> eventBus)
    : Usecase<StartGameRequest>
{
    public override async Task ExecuteAsync(StartGameRequest gameRequest, CancellationToken cancellationToken = default)
    {
        //查
        var readyRoom = await readyRoomRepository.FindByIdAsync(gameRequest.GameId);

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
            .WithMap(new FiveXNineMap());
        var game = builder.Build();

        //存
        await readyRoomRepository.SaveAsync(readyRoom);
        await gameRepository.SaveAsync(game);

        //推
        await eventBus.PublishAsync(readyRoom.DomainEvents, cancellationToken);
    }
}