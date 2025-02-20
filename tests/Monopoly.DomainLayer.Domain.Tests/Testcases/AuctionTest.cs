using Monopoly.DomainLayer.Domain.Builders;
using Monopoly.DomainLayer.Domain.Events;
using Monopoly.DomainLayer.Domain.Maps;

namespace Monopoly.DomainLayer.Domain.Tests.Testcases;

[TestClass]
public class AuctionTest
{
    [TestMethod]
    [Description(
        """
        Given:  玩家A持有1000元
                A1價格為1000元
                玩家A擁有A1
                玩家A正在拍賣A1
                目前無人喊價
        When:   拍賣結算
        Then:   玩家A不再擁有A1
                玩家A持有1700元
        DomainEvent: 拍賣流拍，結束時土地為銀行所有，原地主獲得房地產價格70%，持有金額為 1000 + 700 = 1700
        """)]
    public void 拍賣結算時流拍()
    {
        // Arrange
        var A = new { Id = "A", Money = 1000m };
        var A1 = new { Id = "A1", Price = 1000m };
        var game = 玩家A持有1000元_玩家B持有2000元_玩家A擁有A1_玩家A正在拍賣A1(out var player_a, out var player_b);

        // Act
        game.EndAuction();

        // Assert
        Assert.IsNull(player_a.FindLandContract("A1"));
        Assert.AreEqual(player_a.Money, 1700);

        game.DomainEvents
                .NextShouldBe(new EndAuctionEvent(A.Id, 1700, A1.Id, null, 0))
                .NoMore();
    }

    [TestMethod]
    [Description(
        """
        Given:  玩家A持有1000元
                玩家B持有2000元
                A1價格為1000元
                玩家A擁有A1
                玩家A正在拍賣A1
                玩家B喊價600元
        When:   拍賣結算
        Then:   玩家A持有1600元
                玩家A不再擁有A1
                玩家B持有1400元
                玩家B擁有A1
        DimainEvent: 結算時土地為喊價最高者得，原地主A獲得B喊價600元，A1地主為B，A持有1600元，B持有1400元以及土地A1
        """)]
    public void 拍賣結算時轉移金錢及地契()
    {
        // Arrange
        var map = new SevenXSevenMap();
        var A = new { Id = "A", Money = 1000m };
        var B = new { Id = "B", Money = 2000m };
        var A1 = new { Id = "A1", Price = 1000m };

        var game = new MonopolyBuilder()
            .WithMap(map)
            .WithPlayer(A.Id, p => p.WithMoney(A.Money).WithLandContract(A1.Id, false, 0))
            .WithPlayer(B.Id, p => p.WithMoney(B.Money))
            .WithCurrentPlayer(A.Id, p => p.WithAuction(A1.Id, B.Id, 600m))
            .Build();

        var player_a = game.Players.First(p => p.Id == A.Id);
        var player_b = game.Players.First(p => p.Id == B.Id);

        // Act
        game.EndAuction();

        // Assert
        Assert.IsNull(player_a.FindLandContract("A1"));
        Assert.AreEqual(1600, player_a.Money);
        Assert.AreEqual(1400, player_b.Money);
        Assert.IsNotNull(player_b.FindLandContract("A1"));

        game.DomainEvents
                .NextShouldBe(new EndAuctionEvent(A.Id, 1600, A1.Id, B.Id, 1400))
                .NoMore();
    }

    [TestMethod]
    [Description(
        """
        Given:  玩家A持有1000元
                玩家B持有2000元
                A1價格為1000元
                玩家A擁有A1
                玩家A正在拍賣A1
        When:   玩家B喊價3000元
        Then:   玩家B不能喊價
        DomainEvent: 玩家喊價失敗，最高喊價不變
        """)]
    public void 不能喊出比自己的現金還要大的價錢()
    {
        // Arrange
        var A = new { Id = "A", Money = 1000m };
        var B = new { Id = "B", Money = 2000m };
        var game = 玩家A持有1000元_玩家B持有2000元_玩家A擁有A1_玩家A正在拍賣A1(out var player_a, out var player_b);

        // Act
        game.PlayerBid(player_b.Id, 3000);

        //Assert.ThrowsException<BidException>(() => game.PlayerBid(b.Id, 3000));
        game.DomainEvents
            .NextShouldBe(new PlayerTooPoorToBidEvent(B.Id, B.Money, 3000, 1000))
            .NoMore();


    }

    [TestMethod]
    [Description(
        """
        Given:  玩家A持有1000元
                玩家B持有3000元
                A1價格為1000元
                玩家A擁有A1
                玩家A正在拍賣A1
        When:   玩家B喊價3000元
        Then:   玩家B喊價成功
        DomainEvent: 最高喊價資訊更新
        """)]
    public void 玩家喊價成功()
    {
        // Arrange
        var A = new { Id = "A", Money = 1000m };
        var B = new { Id = "B", Money = 3000m };
        var A1 = new { Id = "A1", Price = 1000m };
        var game = 玩家A持有1000元_玩家B持有2000元_玩家A擁有A1_玩家A正在拍賣A1(out var player_a, out var player_b);
        player_b.Money = 3000;

        // Act
        game.PlayerBid(player_b.Id, 3000);

        game.DomainEvents
            .NextShouldBe(new PlayerBidEvent(B.Id, A1.Id, 3000))
            .NoMore();
    }

    private static MonopolyAggregate 玩家A持有1000元_玩家B持有2000元_玩家A擁有A1_玩家A正在拍賣A1(out Player player_a, out Player player_b)
    {

        var map = new SevenXSevenMap();
        var A = new { Id = "A", Money = 1000m };
        var B = new { Id = "B", Money = 2000m };
        var A1 = new { Id = "A1", Price = 1000m };

        var monopoly = new MonopolyBuilder()
            .WithMap(map)
            .WithPlayer(A.Id, p => p.WithMoney(A.Money).WithLandContract(A1.Id, false, 0))
            .WithPlayer(B.Id, p => p.WithMoney(B.Money))
            .WithCurrentPlayer(A.Id, p => p.WithAuction(A1.Id, null, 1000m))
            .Build();

        player_a = monopoly.Players.First(p => p.Id == A.Id);
        player_b = monopoly.Players.First(p => p.Id == B.Id);

        return monopoly;
    }
}