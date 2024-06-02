using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.DomainLayer.Common;

namespace Monopoly.InterfaceAdapterLayer.Server.Presenters;

public class SignalrDefaultPresenter<TResponse>(IEventBus<DomainEvent> eventBus) : IPresenter<TResponse> 
    where TResponse : CommandResponse
{
    public async Task PresentAsync(TResponse response, CancellationToken cancellationToken)
    {
        await eventBus.PublishAsync(response.Events, cancellationToken);
    }
}