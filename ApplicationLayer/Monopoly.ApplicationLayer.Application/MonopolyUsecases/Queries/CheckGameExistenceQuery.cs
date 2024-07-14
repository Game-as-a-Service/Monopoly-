using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.ApplicationLayer.Application.MonopolyUsecases.Queries.Interfaces;

namespace Monopoly.ApplicationLayer.Application.MonopolyUsecases.Queries;

public class CheckGameExistenceQuery(IGameExistenceInquiry inquiry)
    : Usecase<CheckGameExistenceQuery.Request, CheckGameExistenceQuery.Response>
{
    public record Request(string GameId) : BaseRequest;

    public record Response(bool IsExist) : Common.Response;

    public override Task ExecuteAsync(Request request,
        IPresenter<Response> presenter,
        CancellationToken cancellationToken = default)
    {
        var isExist = inquiry.CheckGameExistence(request.GameId);
        return presenter.PresentAsync(new Response(isExist), cancellationToken);
    }
}