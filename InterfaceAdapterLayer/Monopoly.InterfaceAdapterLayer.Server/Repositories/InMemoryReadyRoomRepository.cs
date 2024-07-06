using Monopoly.ApplicationLayer.Application.ReadyRoomUsecases.Commands;
using Monopoly.DomainLayer.ReadyRoom;

namespace Monopoly.InterfaceAdapterLayer.Server.Repositories;

public class InMemoryReadyRoomRepository : IReadyRoomRepository
{
    private readonly Dictionary<string, ReadyRoomAggregate> _readyRooms = new();
    public Task SaveReadyRoomAsync(ReadyRoomAggregate aggregate)
    {
        _readyRooms[aggregate.Id] = aggregate;
        return Task.CompletedTask;
    }

    public Task<ReadyRoomAggregate> GetReadyRoomAsync(string id)
    {
        var readyRoomAggregate = _readyRooms[id];
        readyRoomAggregate.ClearDomainEvents();
        return Task.FromResult(readyRoomAggregate);
    }
}