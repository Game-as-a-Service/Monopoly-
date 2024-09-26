using Monopoly.ApplicationLayer.Application.MonopolyUsecases.Queries.Interfaces;
using Monopoly.DomainLayer.Domain;

namespace Monopoly.InterfaceAdapterLayer.Server.Repositories.Inquiries;

/// <inheritdoc />
internal class GameExistenceInquiry(FakeInMemoryDatabase<MonopolyAggregate> database) : IGameExistenceInquiry
{
    public async Task<bool> CheckGameExistenceAsync(string gameId)
    {
        var monopoly = await database.FindByIdAsync(gameId);
        return monopoly is not null;
    }
}