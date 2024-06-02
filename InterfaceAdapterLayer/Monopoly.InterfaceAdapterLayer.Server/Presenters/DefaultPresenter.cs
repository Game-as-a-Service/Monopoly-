using Monopoly.ApplicationLayer.Application.Common;

namespace Monopoly.InterfaceAdapterLayer.Server.Presenters;

public class DefaultPresenter<T> : IPresenter<T> where T : class
{
    public T Value { get; set; } = default!;
    public Task PresentAsync(T response, CancellationToken cancellationToken)
    {
        Value = response;
        return Task.CompletedTask;
    }
}
