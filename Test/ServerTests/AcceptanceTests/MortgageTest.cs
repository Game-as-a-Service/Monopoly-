using Server.Hubs;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;
using static ServerTests.Utils;

namespace ServerTests.AcceptanceTests;

[TestClass]
public class MortgageTest
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
        Given:  A 持有 A1，價值 1000元
                A 持有 5000元
        When:   A 抵押 A1
        Then:   A 持有 5000+1000*70% = 5700元
                A 持有 A1
                A1 在 10回合 後收回
        """)]
    public async Task 玩家抵押房地產()
    {
        // Arrange
        var a = new { Id = "A", Money = 5000m };
        var a1 = new { Id = "A1", Price = 1000m };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(a.Id)
            .WithMoney(a.Money)
            .WithLandContract(a1.Id)
            .Build()
        )
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(a.Id).Build());

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerMortgage), "A1");

        // Assert
        // A 抵押房地產
        hub.Verify(nameof(IMonopolyResponses.PlayerMortgageEvent),
            (PlayerMortgageEventArgs e) => e is { PlayerId: "A", PlayerMoney: 5700, LandId: "A1", DeadLine: 10 });
        hub.VerifyNoElseEvent();
    }

    [TestMethod]
    [Description(
        """
        Given:  A 持有 A1，價值 1000元，抵押中
                A 持有 1000元
        When:   A 抵押 A1
        Then:   A 無法再次抵押
                A 持有 1000元
        """)]
    public async Task 玩家不能抵押已抵押房地產()
    {
        // Arrange
        var a = new { Id = "A", Money = 1000m };
        var a1 = new { Id = "A1", Price = 1000m, IsMortgage = true };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(a.Id)
            .WithMoney(a.Money)
            .WithLandContract(a1.Id, inMortgage: a1.IsMortgage)
            .Build()
        )
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(a.Id).Build());

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerMortgage), "A1");

        // Assert
        // A 抵押房地產
        hub.Verify(nameof(IMonopolyResponses.PlayerCannotMortgageEvent),
                                (PlayerCannotMortgageEventArgs e)
                                => e is { PlayerId: "A", PlayerMoney: 1000, LandId: "A1" });
        hub.VerifyNoElseEvent();
    }

    [TestMethod]
    [Description(
        """
        Given:  A 持有 5000元
        When:   A 抵押 A1
        Then:   A 抵押 失敗
        """)]
    public async Task 玩家抵押非自有房地產()
    {
        // Arrange
        var a = new { Id = "A", Money = 5000m };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(a.Id)
            .WithMoney(a.Money)
            .Build()
        )
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(a.Id).Build());

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerMortgage), "A1");

        // Assert
        // A 抵押房地產
        hub.Verify(nameof(IMonopolyResponses.PlayerCannotMortgageEvent),
                    (PlayerCannotMortgageEventArgs e) => e is { PlayerId: "A", PlayerMoney: 5000, LandId: "A1" });
        hub.VerifyNoElseEvent();
    }
}


