namespace Monopoly.DomainLayer.ReadyRoom.Exceptions;

public abstract class AbstractReadyRoomException : Exception
{
    protected AbstractReadyRoomException(string message) : base(message)
    {
    }
}