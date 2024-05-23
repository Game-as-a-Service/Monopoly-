using Application.Common;

namespace Server.Presenters;

public abstract class ValueReturningPresenter<TResult, TResponse> : IPresenter<TResponse>
{
    private readonly TaskCompletionSource<TResult> _tcs = new();

    public Task<TResult> GetResultAsync()
    {
        return _tcs.Task;
    }

    public async Task PresentAsync(TResponse response, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            _tcs.TrySetCanceled(cancellationToken);
            return;
        }

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var result = await TransformAsync(response, cancellationToken).ConfigureAwait(false);
            _tcs.TrySetResult(result);
        }
        catch (OperationCanceledException)
        {
            _tcs.TrySetCanceled(cancellationToken);
            throw; 
        }
        catch (Exception ex)
        {
            _tcs.TrySetException(ex);
        }
    }

    protected abstract Task<TResult> TransformAsync(TResponse response, CancellationToken cancellationToken);
}