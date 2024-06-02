using Monopoly.ApplicationLayer.Application.Common;

namespace Monopoly.InterfaceAdapterLayer.Server.Presenters;

public sealed class NullPresenter<T> : IPresenter<T> where T : Response
{
    public static NullPresenter<T> Instance { get; } = new();
    
    public Task PresentAsync(T response, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}