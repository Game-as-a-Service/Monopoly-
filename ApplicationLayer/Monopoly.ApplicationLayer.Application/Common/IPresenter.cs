namespace Monopoly.ApplicationLayer.Application.Common;

public interface IPresenter<TResponse>
{
    public Task PresentAsync(TResponse response, CancellationToken cancellationToken);
}