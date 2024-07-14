using Monopoly.DomainLayer.Common;
using Monopoly.DomainLayer.Domain.Builders;

namespace Monopoly.InterfaceAdapterLayer.Server.Repositories;

public class FakeInMemoryDatabase<T> where T : AggregateRoot
{
    private readonly Dictionary<string, T> _database = new();
    
    public Task<T?> FindByIdAsync(string id)
    {
        return Task.FromResult(_database.GetValueOrDefault(id));
    }
    
    public Task SaveAsync(T aggregate)
    {
        _database[aggregate.Id] = aggregate;
        return Task.CompletedTask;
    }
}