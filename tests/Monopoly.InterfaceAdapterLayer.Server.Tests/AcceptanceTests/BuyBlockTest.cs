using Monopoly.InterfaceAdapterLayer.Server.Hubs.Monopoly;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;
using static Monopoly.InterfaceAdapterLayer.Server.Tests.Utils;

namespace Monopoly.InterfaceAdapterLayer.Server.Tests.AcceptanceTests;

[TestClass]
public class BuyBlockTest
{
    private MonopolyTestServer server = default!;

    [TestInitialize]
    public void Setup()
    {
        server = new MonopolyTestServer();
    }

    [TestMethod]
    [Description(
        """
        Given:  玩家A資產5000元、沒有房地產、目前輪到A、Ａ在B1上
                F4是空地、購買價1000元
        When:   玩家A購買土地
        Then:   玩家A持有金額為4000
                玩家A持有房地產數量為1
                玩家A持有房地產為B1
        """)]
    public async Task 玩家在空地上可以購買土地()
    {
        // Arrange
        var a = new { Id = "A", Money = 5000m };
        var b1 = new { Id = "B1", Price = 1000m };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder(gameId)
        .WithPlayer(
            new PlayerBuilder(a.Id)
            .WithMoney(a.Money)
            .WithPosition(b1.Id, Direction.Up)
            .Build()
        )
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(a.Id)
            .Build()
        );

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.Requests.PlayerBuyLand(b1.Id);

        // Assert
        // A 購買土地
        hub.FluentAssert.PlayerBuyBlockEvent(new PlayerBuyBlockEventArgs
        {
            PlayerId = a.Id,
            LandId = b1.Id
        });
    }

    [TestMethod]
    [Description(
        """
        Given:  玩家A資產500元,沒有房地產
                目前輪到A,A在C1上
                F4是空地,購買價1000元
        When:   玩家A購買土地
        Then:   顯示錯誤訊息"金額不足"
                玩家A持有金額為500
                玩家A持有房地產數量為0
        """)]
    public async Task 金錢不夠無法購買土地()
    {
        // Arrange
        var a = new { Id = "A", Money = 500m };
        var c1 = new { Id = "C1", Price = 1000m };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(a.Id)
            .WithMoney(a.Money)
            .WithPosition(c1.Id, Direction.Up)
            .Build()
        )
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(a.Id)
            .Build()
        );

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.Requests.PlayerBuyLand(c1.Id);

        // Assert
        // A 購買土地金額不足
        hub.FluentAssert.PlayerBuyBlockInsufficientFundsEvent(new PlayerBuyBlockInsufficientFundsEventArgs
        {
            PlayerId = a.Id,
            LandId = c1.Id,
            Price = 1000
        });
    }

    [TestMethod]
    [Description(
        """
        Given:  玩家A資產5000元、沒有房地產
                玩家B資產5000元、擁有房地產B1
                目前輪到A、Ａ在B1上
        When:   玩家A購買土地B1
        Then:   顯示錯誤訊息“無法購買”
                玩家A持有金額為5000
                玩家A持有房地產數量為0
                玩家B持有金額為5000
                玩家B持有房地產數量為1，持有B1
        """)]
    public async Task 玩家在有地主的土地上不可以購買土地()
    {
        // Arrange
        var a = new { Id = "A", Money = 5000m };
        var b = new { Id = "B", Money = 5000m };
        var b1 = new { Id = "B1", Price = 1000m };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(a.Id)
            .WithMoney(a.Money)
            .WithPosition(b1.Id, Direction.Right)
            .Build()
        )
        .WithPlayer(
            new PlayerBuilder(b.Id)
            .WithMoney(b.Money)
            .WithLandContract(b1.Id)
            .Build()
        )
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(a.Id)
            .Build()
        );

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.Requests.PlayerBuyLand(b1.Id);

        // Assert
        // A 購買土地非空地
        hub.FluentAssert.PlayerBuyBlockOccupiedByOtherPlayerEvent(new PlayerBuyBlockOccupiedByOtherPlayerEventArgs
        {
            PlayerId = a.Id,
            LandId = b1.Id
        });
    }

    // TODO: 目前可以指定要買哪個地，但應該只能買腳下的土地，玩家會發送 BuyLand 而不是 BuyLand(A1)
    [TestMethod]
    [Description(
        """
        Given:  玩家A資產5000元，沒有房地產
                目前輪到A，A在F2上
        When:   玩家A購買土地F4
        Then:   顯示錯誤訊息"必須在購買的土地上才可以購買"
                玩家A持有金額為5000
                玩家A持有房地產數量為0
        """)]
    [Ignore]
    public void 玩家無法購買非腳下的土地()
    {
        // Arrange
        //var A = new { Id = "A", Money = 5000m };


        //const string gameId = "1";
        //var monopolyBuilder = new MonopolyBuilder("1")
        //.WithPlayer(
        //    new PlayerBuilder(A.Id)
        //    .WithMoney(A.Money)
        //    .WithPosition("F2", Direction.Up.ToString())
        //)
        //.WithCurrentPlayer(nameof(A));

        //monopolyBuilder.Save(server);

        //var hub = await server.CreateHubConnectionAsync(gameId, "A");

        //// Act

        //await hub.SendAsync(nameof(MonopolyHub.PlayerBuyLand), gameId, "A", "F4");

        //// Assert
        //// A 購買土地非腳下的土地
        //hub.Verify<string, string>(
        //               nameof(IMonopolyResponses.PlayerBuyBlockMissedLandEvent),
        //                          (playerId, blockId) => playerId == "A" && blockId == "F4");

        //hub.VerifyNoElseEvent();
    }

    [TestMethod]
    [Description(
        """
        Given:  玩家A資產5000元，沒有房地產
                目前輪到A，A在R2上
        When:   玩家A購買土地R4
        Then:   顯示錯誤訊息"無法購買道路"
                玩家A持有金額為5000
                玩家A持有房地產數量為0
        """)]
    public async Task 玩家無法購買道路()
    {
        // Arrange
        var a = new { Id = "A", Money = 5000m };
        var r2 = new { Id = "R2" };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(a.Id)
            .WithMoney(a.Money)
            .WithPosition(r2.Id, Direction.Left)
            .Build()
        )
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(a.Id)
            .Build()
        );

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.Requests.PlayerBuyLand("R2");

        // Assert
        // A 無法購買道路
        hub.FluentAssert.PlayerBuyBlockOccupiedByOtherPlayerEvent(new PlayerBuyBlockOccupiedByOtherPlayerEventArgs
        {
            PlayerId = a.Id,
            LandId = r2.Id
        });
    }
}