using Monopoly.InterfaceAdapterLayer.Server.Hubs.Monopoly;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;
using static Monopoly.InterfaceAdapterLayer.Server.Tests.Utils;

namespace Monopoly.InterfaceAdapterLayer.Server.Tests.AcceptanceTests;

[TestClass]
public class AuctionTest
{
    private MonopolyTestServer server = default!;

    [TestInitialize]

    public void SetUp()
    {
        server = new MonopolyTestServer();
    }

    [TestMethod]
    [Description(
        """
        Given:  A 持有 A1，價值 1000元，有 2棟房
                A 持有 2000元
                B 持有 2000元
                A 拍賣 A1，初始金額為 (1000 + 1000 * 2) * 50% = 1500
        When:   B 喊價 1500
        Then:   B 喊價 成功
        """)]
    public async Task 玩家喊價成功()
    {
        // Arrange
        var a = new { Id = "A", Money = 2000m };
        var b = new { Id = "B", Money = 2000m };
        var a1 = new { Id = "A1", Price = 1000m, HouseCount = 2 };
        var auction = new { LandId = a1.Id };
        const string gameId = "1";

        var monopolyBuilder = new MonopolyBuilder(gameId)
            .WithPlayer(
                new PlayerBuilder(a.Id)
                    .WithMoney(a.Money)
                    .WithLandContract(a1.Id)
                    .Build()
            )
            .WithPlayer(
                new PlayerBuilder(b.Id)
                    .WithMoney(b.Money)
                    .Build()
            )
            .WithCurrentPlayer(
                new CurrentPlayerStateBuilder(a.Id)
                    .WithAuction(auction.LandId, null!, 1500)
                    .Build()
            );

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, b.Id);

        // Act
        await hub.Requests.PlayerBid(1500);

        // Assert
        // B 喊價
        hub.FluentAssert.PlayerBidEvent(new PlayerBidEventArgs
        {
            PlayerId = "B",
            LandId = "A1",
            HighestPrice = 1500
        });
    }

    [TestMethod]
    [Description(
        """
        Given:  A 持有 A1，價值 1000元，有 1棟房
                A 持有 1000元
                B 持有 2000元
                A 拍賣 A1，初始金額為 (1000 + 1000 * 1) * 50% = 1000
        When:   B 喊價 3000
        Then:   B 喊價 失敗
        """)]
    public async Task 玩家不能喊出比自己現金還高的金額()
    {
        // Arrange
        var a = new { Id = "A", Money = 1000m };
        var b = new { Id = "B", Money = 2000m };
        var a1 = new { Id = "A1", Price = 1000m, HouseCount = 1 };
        const string gameId = "1";

        var monopolyBuilder = new MonopolyBuilder(gameId)
            .WithPlayer(
                new PlayerBuilder(a.Id)
                    .WithMoney(a.Money)
                    .WithLandContract(a1.Id)
                    .Build()
            )
            .WithPlayer(
                new PlayerBuilder(b.Id)
                    .WithMoney(b.Money)
                    .Build()
            )
            .WithCurrentPlayer(new CurrentPlayerStateBuilder(a.Id)
                .WithAuction(a1.Id, null!, 1000)
                .Build()
            );

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "B");

        // Act
        await hub.Requests.PlayerBid(3000);

        // Assert
        // B 喊價
        hub.FluentAssert.PlayerTooPoorToBidEvent(new PlayerTooPoorToBidEventArgs
        {
            PlayerId = "B",
            PlayerMoney = 2000,
            BidPrice = 3000,
            HighestPrice = 1000
        });
    }

    [TestMethod]
    [Description(
        """
        Given:  A 持有 A1，價值 1000元，有 1棟房
                A 持有 1000元
                B 持有 2000元
                A 拍賣 A1，初始金額為 (1000 + 1000 * 1) * 50% = 1000
        When:   B 喊價 800
        Then:   B 喊價 失敗
        """)]
    public async Task 玩家不能喊出比當前拍賣金額更低的金額()
    {
        // Arrange
        var a1 = new { Id = "A1", Price = 1000m, HouseCount = 1 };
        var a = new { Id = "A", Money = 1000m, LandContract = new[] { a1 } };
        var b = new { Id = "B", Money = 2000m };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(a.Id)
            .WithMoney(a.Money)
            .WithLandContract("A1")
            .Build()
        )
        .WithPlayer(
            new PlayerBuilder(b.Id)
            .WithMoney(b.Money)
            .Build()
        )
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(a.Id)
            .WithAuction(a1.Id, null!, 1000)
            .Build()
        );

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "B");

        // Act
        await hub.Requests.PlayerBid(800);

        // Assert
        // B 喊價
        hub.FluentAssert.PlayerBidFailEvent(new PlayerBidFailEventArgs
        {
            PlayerId = "B",
            LandId = "A1",
            BidPrice = 800,
            HighestPrice = 1000
        });
    }

    [TestMethod]
    [Description(
        """
        Given:  A 持有 A1，價值 1000元，有 1棟房
                A 持有 2000元
                A 拍賣 A1，初始金額為 (1000 + 1000 * 1) * 50% = 1000
        When:   A 拍賣房地產 流拍
        Then:   A 持有金額為 2000 + (1000 + 1000 * 1) * 70% = 3400
                A1 為系統所有
        """)]
    public async Task 拍賣結算時流拍系統會以房地產價值的七成收購()
    {
        // Arrange
        var a = new { Id = "A", Money = 2000m };
        var a1 = new { Id = "A1", Price = 1000m, HouseCount = 1 };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(a.Id)
            .WithMoney(a.Money)
            .WithLandContract(a1.Id)
            .Build()
        )
        .WithCurrentPlayer(
            new CurrentPlayerStateBuilder(a.Id)
            .WithAuction(a1.Id, null!, 1000)
            .Build()
        )
        .WithLandHouse(a1.Id, a1.HouseCount);

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.Requests.EndAuction();

        // Assert
        // 流拍
        // 在流拍中，土地為系統所有，OwnerMoney值回傳0
        hub.FluentAssert.EndAuctionEvent(new EndAuctionEventArgs
        {
            PlayerId = "A",
            PlayerMoney = 3400,
            LandId = "A1",
            OwnerId = null,
            OwnerMoney = 0
        });
    }

    [TestMethod]
    [Description(
        """
        Given:  A 持有 A1，價值 1000元
                A 持有 1000元
                B 持有 2000元
                A 拍賣 A1，初始金額為 1000 * 50% = 500
                B 喊價 600元
        When:   拍賣結算
        Then:   A 持有金額為 1000 + 600 = 1600
                B 持有金額為 2000 - 600 = 1400
                A1 為 B 所有
        """)]
    public async Task 拍賣結算時轉移金錢及地契()
    {
        // Arrange
        var a1 = new { Id = "A1", Price = 1000m, HouseCount = 1 };
        var a = new { Id = "A", Money = 1000m, LandContract = new[] { a1 } };
        var b = new { Id = "B", Money = 2000m };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(a.Id)
            .WithMoney(a.Money)
            .WithLandContract(a1.Id)
            .Build()
        )
        .WithPlayer(
            new PlayerBuilder(b.Id)
            .WithMoney(b.Money)
            .Build()
        )
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(a.Id)
            .WithAuction(landId: a1.Id, highestBidderId: b.Id, highestPrice: 600)
            .Build()
        );

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.Requests.EndAuction();

        // Assert
        // 拍賣結算
        hub.FluentAssert.EndAuctionEvent(new EndAuctionEventArgs
        {
            PlayerId = "A",
            PlayerMoney = 1600,
            LandId = "A1",
            OwnerId = "B",
            OwnerMoney = 1400
        });
    }

    [TestMethod]
    [Description(
        """
        Given:  A 持有 A1，價值 1000元，有 2棟房
                A 持有 2000元
                B 持有 2000元
                A 拍賣 A1，初始金額為 (1000 + 1000 * 2) * 50% = 1500
        When:   A 喊價 1500
        Then:   A 喊價 失敗
        """)]
    public async Task 拍賣土地的玩家不能自己喊價()
    {
        // Arrange
        var a1 = new { Id = "A1", Price = 1000m, HouseCount = 2 };
        var a = new { Id = "A", Money = 2000m, LandContract = new[] { a1 } };
        var b = new { Id = "B", Money = 2000m };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(a.Id)
            .WithMoney(a.Money)
            .WithLandContract(a1.Id)
            .Build()
        )
        .WithPlayer(
            new PlayerBuilder(b.Id)
            .WithMoney(b.Money)
            .Build()
        )
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(a.Id)
            .WithAuction(landId: a1.Id, null, 1500)
            .Build()
        );

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.Requests.PlayerBid(1500);

        // Assert
        // A 喊價
        hub.FluentAssert.CurrentPlayerCannotBidEvent(new CurrentPlayerCannotBidEventArgs
        {
            PlayerId = "A"
        });
    }
}