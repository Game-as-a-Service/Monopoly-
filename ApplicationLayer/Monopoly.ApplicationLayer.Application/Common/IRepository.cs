using Monopoly.DomainLayer.Domain;

namespace Monopoly.ApplicationLayer.Application.Common;

public interface IRepository
{
    public MonopolyAggregate FindGameById(string id);
    public string[] GetRooms();
    public bool IsExist(string id);
    public string Save(MonopolyAggregate monopoly);
}

public interface ICommandRepository : IRepository
{
}

public interface IQueryRepository : IRepository
{
}