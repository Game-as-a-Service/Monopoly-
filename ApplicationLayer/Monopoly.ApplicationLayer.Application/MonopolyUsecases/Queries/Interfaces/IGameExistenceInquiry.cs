namespace Monopoly.ApplicationLayer.Application.MonopolyUsecases.Queries.Interfaces;

public interface IGameExistenceInquiry
{
    bool CheckGameExistence(string gameId);
}