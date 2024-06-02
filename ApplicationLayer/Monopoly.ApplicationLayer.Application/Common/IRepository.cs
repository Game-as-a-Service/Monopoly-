using Monopoly.ApplicationLayer.Application.DataModels;

namespace Monopoly.ApplicationLayer.Application.Common;

public interface IRepository
{
    public MonopolyDataModel FindGameById(string id);
    public string[] GetRooms();
    public bool IsExist(string id);
    public string Save(MonopolyDataModel monopolyDataModel);
}

public interface ICommandRepository : IRepository
{
}

public interface IQueryRepository : IRepository
{
}