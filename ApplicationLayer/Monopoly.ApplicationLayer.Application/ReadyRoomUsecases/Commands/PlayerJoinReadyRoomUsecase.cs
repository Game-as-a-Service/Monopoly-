﻿using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.DomainLayer.Common;
using Monopoly.DomainLayer.ReadyRoom;

namespace Monopoly.ApplicationLayer.Application.ReadyRoomUsecases.Commands;

public record PlayerJoinReadyRoomRequest(string RoomId, string PlayerId) : BaseRequest;

public class PlayerJoinReadyRoomUsecase(IRepository<ReadyRoomAggregate> repository, IEventBus<DomainEvent> eventBus)
    : Usecase<PlayerJoinReadyRoomRequest>
{
    public override async Task ExecuteAsync(PlayerJoinReadyRoomRequest request,
        CancellationToken cancellationToken = default)
    {
        // 查
        var readyRoom = await repository.FindByIdAsync(request.RoomId);

        // 改
        readyRoom.PlayerJoin(request.PlayerId);

        // 存
        await repository.SaveAsync(readyRoom);

        // 推
        await eventBus.PublishAsync(readyRoom.DomainEvents, cancellationToken);
    }
}