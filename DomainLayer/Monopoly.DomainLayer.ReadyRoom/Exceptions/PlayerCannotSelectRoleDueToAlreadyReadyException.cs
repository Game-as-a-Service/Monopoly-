namespace Monopoly.DomainLayer.ReadyRoom.Exceptions;

public sealed class PlayerCannotSelectRoleDueToAlreadyReadyException()
    : AbstractReadyRoomException("Player is already ready.");