using Monopoly.DomainLayer.Domain.Builders;
using Monopoly.DomainLayer.Domain.Maps;

namespace Monopoly.DomainLayer.Domain.Tests.Testcases;

[TestClass]
public class BankruptTest
{
    [Ignore]
    [TestMethod]
    public void 玩家A_A沒錢沒房__更新玩家A的狀態__玩家A的狀態為破產()
    {
        // Arrange
        Map map = new SevenXSevenMap();
        var A = new { Id = "a", Money = 0m, CurrentBlockId = "Start", CurrentDirection = "Right" };


        var monopoly = new MonopolyBuilder()
            .WithMap(map)
            .WithPlayer(A.Id, p => p.WithMoney(A.Money)
                                    .WithPosition(A.CurrentBlockId, A.CurrentDirection))
            .Build();
        monopoly.Initial();

        // Act
        //monopoly.UpdatePlayerState(player_a);

        // Assert
        //Assert.AreEqual(PlayerState.Bankrupt, player_a.State);
    }
}