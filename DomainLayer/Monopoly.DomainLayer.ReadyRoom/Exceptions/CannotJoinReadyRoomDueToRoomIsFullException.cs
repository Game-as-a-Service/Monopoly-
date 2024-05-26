namespace Monopoly.DomainLayer.ReadyRoom.Exceptions;

public class CannotJoinReadyRoomDueToRoomIsFullException() : AbstractReadyRoomException("Cannot join ready room due to room is full");