using Monopoly.DomainLayer.Common;
using Monopoly.DomainLayer.Domain.Builders;

namespace Monopoly.InterfaceAdapterLayer.Server.Repositories;

public class FakeInMemoryDatabase<T> where T : AggregateRoot
{
    private readonly Dictionary<string, T> _database = new();
    
    public T? FindById(string id)
    {
        return _database.GetValueOrDefault(id);
    }
    
    public void Save(T aggregate)
    {
        _database[aggregate.Id] = aggregate;
    }
}