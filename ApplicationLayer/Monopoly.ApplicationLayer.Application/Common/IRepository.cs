using Monopoly.DomainLayer.Common;

namespace Monopoly.ApplicationLayer.Application.Common;

public interface IRepository<TAggregate> where TAggregate : AggregateRoot
{
    /// <summary>
    /// Find an aggregate by its id
    /// </summary>
    /// <param name="id">aggregate id</param>
    /// <returns>aggregate</returns>
    public Task<TAggregate> FindByIdAsync(string id);
    
    /// <summary>
    /// Save an aggregate, return the aggregate id
    /// </summary>
    /// <param name="aggregate">aggregate</param>
    /// <returns>aggregate id</returns>
    public Task<string> SaveAsync(TAggregate aggregate);
}