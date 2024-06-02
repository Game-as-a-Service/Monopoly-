using Monopoly.DomainLayer.Common;

namespace Application.Common;

public abstract class Usecase<TRequest, TResponse>()
    where TRequest : BaseRequest where TResponse : Response
{
    public abstract Task ExecuteAsync(TRequest request, IPresenter<TResponse> presenter,
        CancellationToken cancellationToken = default);
}

public abstract class CommandUsecase<TRequest, TResponse>(ICommandRepository repository, IEventBus<DomainEvent> eventBus)
    : Usecase<TRequest, TResponse>()
    where TRequest : GameRequest where TResponse : Response
{
    protected ICommandRepository Repository { get; } = repository;
    protected IEventBus<DomainEvent> EventBus { get; } = eventBus;
}

public abstract class QueryUsecase<TRequest, TResponse>(ICommandRepository repository)
    where TRequest : GameRequest
{
    protected ICommandRepository Repository { get; } = repository;

    public abstract Task ExecuteAsync(TRequest request, IPresenter<TResponse> presenter);
}
