using Application.Common;

namespace Server.Presenters;

public sealed class NullPresenter<T> : IPresenter<T> where T : Response
{
    public static NullPresenter<T> Instance { get; } = new();
    
    public Task PresentAsync(T response)
    {
        return Task.CompletedTask;
    }
}