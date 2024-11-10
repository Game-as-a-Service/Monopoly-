using System.Linq.Expressions;
using Monopoly.DomainLayer.Common;

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

    public IEnumerable<T> FindAsync(Expression<Func<T, bool>> expression)
    {
        var result = _database.Values.Where(expression.Compile());
        return result;
    }
}
