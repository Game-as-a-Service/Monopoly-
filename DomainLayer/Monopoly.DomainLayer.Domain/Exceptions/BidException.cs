using System.Runtime.Serialization;

namespace Monopoly.DomainLayer.Domain.Exceptions
{
    [Serializable]
    public class BidException : Exception
    {
        public BidException()
        {
        }

        public BidException(string? message) : base(message)
        {
        }

        public BidException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected BidException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}