using Monopoly.ApplicationLayer.Application.ReadyRoomUsecases.Queries.Interfaces;
using Monopoly.DomainLayer.ReadyRoom;

namespace Monopoly.InterfaceAdapterLayer.Server.Repositories.Inquiries;

public class GetReadyRoomAllPlayerInfosInquiry(FakeInMemoryDatabase<ReadyRoomAggregate> database)
    : IGetReadyRoomAllPlayerInfosInquiry
{
    public Task<IEnumerable<IGetReadyRoomAllPlayerInfosInquiry.PlayerInfos>> GetReadyRoomAllPlayerInfosAsync(
        string gameId)
    {
        var readyRooms = database.FindAsync(x => x.GameId == gameId);

        var readyRoomAggregates = readyRooms.ToList();
        var readyRoom = readyRoomAggregates.FirstOrDefault();
        
        if (readyRoom == null)
        {
            throw new InvalidOperationException("ReadyRoom not found");
        }

        return Task.FromResult(readyRoom.Players.Select(player => new IGetReadyRoomAllPlayerInfosInquiry.PlayerInfos(
            player.Id,
            player.Name,
            player.Location.ToString(),
            player.RoleId
        )));
    }
}