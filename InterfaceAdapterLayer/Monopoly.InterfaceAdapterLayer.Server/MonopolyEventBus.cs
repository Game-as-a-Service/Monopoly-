using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.DomainLayer.Common;
using Monopoly.InterfaceAdapterLayer.Server.Common;

namespace Monopoly.InterfaceAdapterLayer.Server;

internal class MonopolyEventBus(IEnumerable<IMonopolyEventHandler> handlers) : IEventBus<DomainEvent>
{
    private readonly Dictionary<Type, IMonopolyEventHandler> _handlers = handlers.ToDictionary(h => h.EventType, h => h);

    public async Task PublishAsync(IEnumerable<DomainEvent> events, CancellationToken cancellationToken)
    {
        foreach (var e in events)
        {
            var handler = GetHandler(e);
            await handler!.HandleAsync(e);
        }
    }

    private IMonopolyEventHandler GetHandler(DomainEvent e)
    {
        var type = e.GetType();
        if (!_handlers.TryGetValue(type, out var handler))
        {
            throw new InvalidOperationException($"Handler for {type} not registered");
        }
        return handler;
    }
}