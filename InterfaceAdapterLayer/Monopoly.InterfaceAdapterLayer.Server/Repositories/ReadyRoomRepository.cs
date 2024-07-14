using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.DomainLayer.ReadyRoom;

namespace Monopoly.InterfaceAdapterLayer.Server.Repositories;

/// <inheritdoc />
public class ReadyRoomRepository(FakeInMemoryDatabase<ReadyRoomAggregate> database) : IRepository<ReadyRoomAggregate>
{
    public async Task<string> SaveAsync(ReadyRoomAggregate aggregate)
    {
        await database.SaveAsync(aggregate);
        return aggregate.Id;
    }

    public async Task<ReadyRoomAggregate> FindByIdAsync(string id)
    {
        var readyRoomAggregate = await database.FindByIdAsync(id);
        if (readyRoomAggregate is null)
        {
            throw new ArgumentException($"ReadyRoom with id {id} not found");
        }
        readyRoomAggregate.ClearDomainEvents();
        return readyRoomAggregate;
    }
}