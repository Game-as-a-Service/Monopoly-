using Monopoly.DomainLayer.Common;

namespace Monopoly.ApplicationLayer.Application.Common;

public interface IEventBus<TEvent> where TEvent : DomainEvent
{
    public Task PublishAsync(IEnumerable<TEvent> events, CancellationToken cancellationToken);
}