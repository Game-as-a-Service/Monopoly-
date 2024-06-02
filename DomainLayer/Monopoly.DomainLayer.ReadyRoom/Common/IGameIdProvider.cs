namespace Monopoly.DomainLayer.ReadyRoom.Common;

public interface IGameIdProvider
{
    string GetGameId();
}

public sealed class GameIdProvider : IGameIdProvider
{
    public string GetGameId() => Guid.NewGuid().ToString();
}