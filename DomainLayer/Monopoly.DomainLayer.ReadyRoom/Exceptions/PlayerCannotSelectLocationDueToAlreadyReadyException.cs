namespace Monopoly.DomainLayer.ReadyRoom.Exceptions;

public class PlayerCannotSelectLocationDueToAlreadyReadyException()
    : AbstractReadyRoomException("Player cannot select location due to already ready");