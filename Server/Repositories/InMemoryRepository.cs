using Application.Common;
using Application.DataModels;

namespace Server.Repositories;

public class InMemoryRepository : ICommandRepository, IQueryRepository
{
    private readonly Dictionary<string, Application.DataModels.MonopolyDataModel> games = new();

    public Application.DataModels.MonopolyDataModel FindGameById(string id)
    {
        games.TryGetValue(id, out var game);
        if (game == null)
        {
            throw new GameNotFoundException(id);
        }

        return game;
    }

    public string[] GetRooms()
    {
        return games.Keys.ToArray();
    }

    public bool IsExist(string id)
    {
        return games.ContainsKey(id);
    }

    public string Save(Application.DataModels.MonopolyDataModel monopolyDataModel)
    {
        var id = GetGameId(monopolyDataModel.Id);
        var game = monopolyDataModel with { Id = id };
        games[game.Id] = game;
        return game.Id;
    }

    private string GetGameId(string gameId)
    {
        return string.IsNullOrWhiteSpace(gameId) ? (games.Count + 1).ToString() : gameId;
    }
}