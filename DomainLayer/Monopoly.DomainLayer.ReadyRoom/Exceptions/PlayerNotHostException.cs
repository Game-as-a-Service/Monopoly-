namespace Monopoly.DomainLayer.ReadyRoom.Exceptions;

public class PlayerNotHostException(string playerId) : AbstractReadyRoomException($"Player {playerId} is not host.");