using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.ApplicationLayer.Application.MonopolyUsecases.Commands;
using Monopoly.DomainLayer.Common;
using Monopoly.DomainLayer.Domain;

#pragma warning disable CS9107 // Parameter is captured into the state of the enclosing type and its value is also passed to the base constructor. The value might be captured by the base class as well.

namespace Monopoly.InterfaceAdapterLayer.Server.Tests.Usecases;

public class MockPlayerRollDiceUsecase(IRepository<MonopolyAggregate> repository,
                                       IEventBus<DomainEvent> eventBus,
                                       MockDiceService mockDiceService)
    : PlayerRollDiceUsecase(repository, eventBus)
{
    public override async Task ExecuteAsync(PlayerRollDiceRequest request,
        IPresenter<PlayerRollDiceResponse> presenter, CancellationToken cancellationToken = default)
    {
        //查
        var game = repository.FindById(request.GameId);

        // Mock Dice
        game.Dices = mockDiceService.Dices;

        //改
        game.PlayerRollDice(request.PlayerId);

        //存
        repository.Save(game);

        //推
        await presenter.PresentAsync(new PlayerRollDiceResponse(game.DomainEvents), cancellationToken);
    }
}