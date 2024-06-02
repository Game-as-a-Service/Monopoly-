using Monopoly.DomainLayer.ReadyRoom;

namespace Monopoly.ApplicationLayer.Application.Usecases.ReadyRoom;

public interface IReadyRoomRepository
{
    /// <summary>
    /// Save ready room aggregate to repository.
    /// </summary>
    /// <param name="aggregate">Ready room aggregate</param>
    /// <returns></returns>
    Task SaveReadyRoomAsync(ReadyRoomAggregate aggregate);

    /// <summary>
    /// Gat ready room from repository with identifier.
    /// </summary>
    /// <param name="id">Identifier of ready room.</param>
    /// <returns>Ready room aggregate.</returns>
    Task<ReadyRoomAggregate> GetReadyRoomAsync(string id);
}