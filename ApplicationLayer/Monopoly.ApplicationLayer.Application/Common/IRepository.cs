using Monopoly.DomainLayer.Common;

namespace Monopoly.ApplicationLayer.Application.Common;

public interface IRepository<TAggregate> where TAggregate : AggregateRoot
{
    public TAggregate FindById(string id);
    public string[] GetRooms();
    public bool IsExist(string id);
    public string Save(TAggregate aggregate);
}