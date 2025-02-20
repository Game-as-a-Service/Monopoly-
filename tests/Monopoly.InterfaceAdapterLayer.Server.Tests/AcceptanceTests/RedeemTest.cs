using Monopoly.InterfaceAdapterLayer.Server.Hubs.Monopoly;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;
using static Monopoly.InterfaceAdapterLayer.Server.Tests.Utils;

namespace Monopoly.InterfaceAdapterLayer.Server.Tests.AcceptanceTests;

[TestClass]
public class RedeemTest
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
                A 持有 5000元
                A1 抵押中
        When:   A 贖回 A1
        Then:   A 持有 5000 - 3000 = 2000元
                A 持有 A1
                A1 不在抵押狀態
        """)]
    public async Task 玩家贖回房地產()
    {
        // Arrange
        var a = new { Id = "A", Money = 5000m };
        var a1 = new { Id = "A1", House = 2, Price = 1000m, IsMortgage = true };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(a.Id)
            .WithMoney(a.Money)
            .WithLandContract(a1.Id, a1.IsMortgage)
            .Build()
        )
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(a.Id).Build())
        .WithLandHouse(a1.Id, a1.House);

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.Requests.PlayerRedeem("A1");

        // Assert
        // A 贖回房地產
        hub.FluentAssert.PlayerRedeemEvent(new PlayerRedeemEventArgs
        {
            PlayerId = a.Id,
            LandId = a1.Id,
            PlayerMoney = 2000
        });
    }

    [TestMethod]
    [Description(
        """
        Given:  A 持有 A1，價值 1000元，有 2棟房
                A 持有 2000元
                A1 抵押中
        When:   A 贖回 A1
        Then:   A 持有 2000元
                A 持有 A1
                A1 抵押中
        """)]
    public async Task 玩家餘額不足以贖回房地產()
    {
        // Arrange
        var a = new { Id = "A", Money = 2000m };
        var a1 = new { Id = "A1", House = 2, Price = 1000m, IsMortgage = true };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(a.Id)
            .WithMoney(a.Money)
            .WithLandContract(a1.Id, a1.IsMortgage)

            .Build()
        )
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(a.Id).Build())
        .WithLandHouse(a1.Id, a1.House);

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.Requests.PlayerRedeem("A1");

        // Assert
        // A 贖回房地產
        hub.FluentAssert.PlayerTooPoorToRedeemEvent(new PlayerTooPoorToRedeemEventArgs
        {
            PlayerId = a.Id,
            LandId = a1.Id,
            PlayerMoney = 2000,
            RedeemPrice = 3000
        });
    }
}