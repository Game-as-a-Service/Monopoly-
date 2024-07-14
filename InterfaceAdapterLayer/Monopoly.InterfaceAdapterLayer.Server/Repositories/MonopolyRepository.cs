using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.DomainLayer.Domain;

namespace Monopoly.InterfaceAdapterLayer.Server.Repositories;

public class MonopolyRepository(FakeInMemoryDatabase<MonopolyAggregate> database) : IRepository<MonopolyAggregate>
{
    public MonopolyAggregate FindById(string id)
    {
        var game = database.FindById(id);
        if (game == null)
        {
            throw new GameNotFoundException(id);
        }

        return game;
    }

    public string Save(MonopolyAggregate monopoly)
    {
        var id = GetGameId(monopoly.Id);
        var newMonopoly = monopoly.ToApplication() with { Id = id };
        
        database.Save(newMonopoly.ToDomain());
        return monopoly.Id;
    }

    private static string GetGameId(string gameId)
    {
        return string.IsNullOrWhiteSpace(gameId) ? Guid.NewGuid().ToString() : gameId;
    }
}