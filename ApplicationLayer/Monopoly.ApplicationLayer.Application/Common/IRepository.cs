using Monopoly.DomainLayer.Common;

namespace Monopoly.ApplicationLayer.Application.Common;

public interface IRepository<TAggregate> where TAggregate : AggregateRoot
{
    public Task<TAggregate> FindByIdAsync(string id);
    public Task<string> SaveAsync(TAggregate aggregate);
}