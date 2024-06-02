using SharedLibrary.ResponseArgs.ReadyRoom;

namespace SharedLibrary;

public interface IReadyRoomResponses
{
    Task GameStartedEvent(GameStartedEventArgs args);
    Task PlayerReadyEvent(PlayerReadyEventArgs args);
    Task PlayerSelectLocationEvent(PlayerSelectLocationEventArgs args);
    Task PlayerSelectRoleEvent(PlayerSelectRoleEventArgs args);
    Task PlayerJoinReadyRoomEvent(PlayerJoinReadyRoomEventArgs playerJoinReadyRoomEventArgs);
}