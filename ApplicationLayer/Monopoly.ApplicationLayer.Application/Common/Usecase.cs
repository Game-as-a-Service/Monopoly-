namespace Monopoly.ApplicationLayer.Application.Common;

public abstract class Usecase<TRequest, TResponse> where TRequest : BaseRequest where TResponse : Response
{
    public abstract Task ExecuteAsync(TRequest request, IPresenter<TResponse> presenter,
        CancellationToken cancellationToken = default);
}

public abstract class Usecase<TRequest> where TRequest : BaseRequest
{
    public abstract Task ExecuteAsync(TRequest request, CancellationToken cancellationToken = default);
}