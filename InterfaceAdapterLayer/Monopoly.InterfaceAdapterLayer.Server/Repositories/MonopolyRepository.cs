using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.DomainLayer.Domain;

namespace Monopoly.InterfaceAdapterLayer.Server.Repositories;

/// <inheritdoc />
public class MonopolyRepository(FakeInMemoryDatabase<MonopolyAggregate> database) : IRepository<MonopolyAggregate>
{
    public async Task<MonopolyAggregate> FindByIdAsync(string id)
    {
        var game = await database.FindByIdAsync(id);
        if (game == null)
        {
            throw new GameNotFoundException(id);
        }

        return game;
    }

    public async Task<string> SaveAsync(MonopolyAggregate monopoly)
    {
        var id = GetGameId(monopoly.Id);
        var newMonopoly = monopoly.ToApplication() with { Id = id };
        
        await database.SaveAsync(newMonopoly.ToDomain());
        return monopoly.Id;
    }

    private static string GetGameId(string gameId)
    {
        return string.IsNullOrWhiteSpace(gameId) ? Guid.NewGuid().ToString() : gameId;
    }
}