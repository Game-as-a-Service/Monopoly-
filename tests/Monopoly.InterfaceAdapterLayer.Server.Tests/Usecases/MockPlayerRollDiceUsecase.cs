using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.ApplicationLayer.Application.Usecases;
using Monopoly.DomainLayer.Common;

namespace Monopoly.InterfaceAdapterLayer.Server.Tests.Usecases;

public class MockPlayerRollDiceUsecase(ICommandRepository repository,
                                       IEventBus<DomainEvent> eventBus,
                                       MockDiceService mockDiceService)
    : PlayerRollDiceUsecase(repository, eventBus)
{
    public override async Task ExecuteAsync(PlayerRollDiceRequest request,
        IPresenter<PlayerRollDiceResponse> presenter, CancellationToken cancellationToken)
    {
        //查
        var game = Repository.FindGameById(request.GameId).ToDomain();

        // Mock Dice
        game.Dices = mockDiceService.Dices;

        //改
        game.PlayerRollDice(request.PlayerId);

        //存
        Repository.Save(game);

        //推
        await presenter.PresentAsync(new PlayerRollDiceResponse(game.DomainEvents), cancellationToken);
    }
}