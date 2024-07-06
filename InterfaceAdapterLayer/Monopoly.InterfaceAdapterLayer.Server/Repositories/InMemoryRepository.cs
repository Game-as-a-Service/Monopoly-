using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.ApplicationLayer.Application.DataModels;
using Monopoly.DomainLayer.Domain;

namespace Monopoly.InterfaceAdapterLayer.Server.Repositories;

public class InMemoryRepository : IRepository
{
    private readonly Dictionary<string, MonopolyDataModel> games = new();

    public MonopolyAggregate FindGameById(string id)
    {
        games.TryGetValue(id, out var game);
        if (game == null)
        {
            throw new GameNotFoundException(id);
        }

        return game.ToDomain();
    }

    public string[] GetRooms()
    {
        return games.Keys.ToArray();
    }

    public bool IsExist(string id)
    {
        return games.ContainsKey(id);
    }

    public string Save(MonopolyAggregate monopoly)
    {
        var id = GetGameId(monopoly.Id);
        var game = monopoly.ToApplication() with { Id = id };
        games[game.Id] = game;
        return game.Id;
    }

    private string GetGameId(string gameId)
    {
        return string.IsNullOrWhiteSpace(gameId) ? (games.Count + 1).ToString() : gameId;
    }
}