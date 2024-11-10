namespace Monopoly.ApplicationLayer.Application.ReadyRoomUsecases.Queries.Interfaces;

public interface IGetReadyRoomAllPlayerInfosInquiry
{
    Task<IEnumerable<PlayerInfos>> GetReadyRoomAllPlayerInfosAsync(string gameId);
    
    public sealed record PlayerInfos(string Id, string Name, string Color, string Role);
}