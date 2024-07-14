using Monopoly.DomainLayer.ReadyRoom;

namespace Monopoly.ApplicationLayer.Application.ReadyRoomUsecases.Commands;

public interface IReadyRoomRepository
{
    /// <summary>
    /// Save ready room aggregate to repository.
    /// </summary>
    /// <param name="aggregate">Ready room aggregate</param>
    /// <returns></returns>
    Task SaveAsync(ReadyRoomAggregate aggregate);

    /// <summary>
    /// Gat ready room from repository with identifier.
    /// </summary>
    /// <param name="id">Identifier of ready room.</param>
    /// <returns>Ready room aggregate.</returns>
    Task<ReadyRoomAggregate> FindByIdAsync(string id);
}