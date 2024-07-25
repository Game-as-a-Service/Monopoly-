using Monopoly.InterfaceAdapterLayer.Server.Hubs.Monopoly;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;
using static Monopoly.InterfaceAdapterLayer.Server.Tests.Utils;

namespace Monopoly.InterfaceAdapterLayer.Server.Tests.AcceptanceTests;

[TestClass]
public class BuildHouseTest
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
        Given:  A 持有 A1
                A1 有 1間房子，購買價 1000元
                A 持有 2000元，在 A1 上
        When:   A 蓋房子
        Then:   A 持有 1000元
                A1 有 2間房子
        """)]
    public async Task 玩家在自己的土地蓋房子()
    {
        // Arrange
        var a = new { Id = "A", Money = 2000m };
        var a1 = new { Id = "A1", House = 1, Price = 1000m };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(a.Id)
            .WithMoney(a.Money)
            .WithPosition(a1.Id, Direction.Right)
            .WithLandContract(a1.Id)
            .Build()
        )
        .WithMockDice(new[] { 1 })
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(a.Id)
            .Build()
        )
        .WithLandHouse(a1.Id, a1.House);

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.Requests.PlayerBuildHouse();

        // Assert
        // A 蓋房子
        hub.FluentAssert.PlayerBuildHouseEvent(new PlayerBuildHouseEventArgs
        {
            PlayerId = "A",
            LandId = "A1",
            PlayerMoney = 1000,
            HouseCount = 2
        });
    }

    [TestMethod]
    [Description(
        """
        Given:  A 持有 A1
                A1 有 5間房子，購買價 1000元
                A 持有 2000元，在 A1 上
        When:   A 蓋房子
        Then:   A 不能蓋房子
        """)]
    public async Task 土地最多蓋5間房子()
    {
        // Arrange
        var a = new { Id = "A", Money = 2000m };
        var a1 = new { Id = "A1", House = 5, Price = 1000m };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(a.Id)
            .WithMoney(a.Money)
            .WithPosition(a1.Id, Direction.Right)
            .WithLandContract(a1.Id)
            .Build()
        )
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(a.Id).Build())
        .WithLandHouse(a1.Id, a1.House);

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.Requests.PlayerBuildHouse();

        // Assert
        // A 蓋房子
        hub.FluentAssert.HouseMaxEvent(new HouseMaxEventArgs
        {
            PlayerId = "A",
            LandId = "A1",
            HouseCount = 5
        });
    }

    [TestMethod]
    [Description(
        """
        Given:  A 持有 車站1，購買價 1000元
                A 持有 1000元，在 車站1 上
        When:   A 蓋房子
        Then:   A 不能蓋房子
        """)]
    public async Task 車站不能蓋房子()
    {
        // Arrange
        var a = new { Id = "A", Money = 1000m };
        var station1 = new { Id = "Station1", Price = 1000m };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(a.Id)
            .WithMoney(a.Money)
            .WithPosition(station1.Id, Direction.Right)
            .WithLandContract(station1.Id)
            .Build()
        )
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(a.Id).Build());

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.Requests.PlayerBuildHouse();

        // Assert
        // A 蓋房子
        hub.FluentAssert.PlayerCannotBuildHouseEvent(new PlayerCannotBuildHouseEventArgs
        {
            PlayerId = "A",
            LandId = "Station1"
        });
    }

    [TestMethod]
    [Description(
        """
        Given:  A 持有 A1，購買價 1000元
                A 持有 10000元，在 A1 上
                本回合已經蓋過房子了
        When:   A 蓋房子
        Then:   A 不能蓋房子
        """)]
    public async Task 一回合內玩家不能重複蓋房子()
    {
        // Arrange
        var a = new { Id = "A", Money = 10000m };
        var a1 = new { Id = "A1", Price = 1000m };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(a.Id)
            .WithMoney(a.Money)
            .WithPosition(a1.Id, Direction.Right)
            .WithLandContract(a1.Id)
            .Build()
        )
        .WithCurrentPlayer(
            new CurrentPlayerStateBuilder(a.Id)
            .WithUpgradeLand()
            .Build()
        );

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.Requests.PlayerBuildHouse();

        // Assert
        // A 蓋房子
        hub.FluentAssert.PlayerCannotBuildHouseEvent(new PlayerCannotBuildHouseEventArgs
        {
            PlayerId = "A",
            LandId = "A1"
        });
    }

    [TestMethod]
    [Description(
        """
        Given:  A 持有 A1，購買價 1000元
                A 持有 1000元，在 A1 上
                A1 於本回合購買 
        When:   A 蓋房子
        Then:   A 不能蓋房子
        """)]
    public async Task 一回合內玩家買土地後不能馬上蓋房子()
    {
        // Arrange
        var a = new { Id = "A", Money = 1000m };
        var a1 = new { Id = "A1", Price = 1000m };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(a.Id)
            .WithMoney(a.Money)
            .WithPosition(a1.Id, Direction.Right)
            .WithLandContract(a1.Id)
            .Build()
        )
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(a.Id)
            .WithBoughtLand()
            .Build()
        );

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.Requests.PlayerBuildHouse();

        // Assert
        hub.FluentAssert.PlayerCannotBuildHouseEvent(new PlayerCannotBuildHouseEventArgs
        {
            PlayerId = "A",
            LandId = "A1"
        });
    }

    [TestMethod]
    [Description("""
                Given:  目前玩家在A2
                        玩家持有A2
                        A2房子不足5間
                        A2抵押中
                When:   A 蓋房子
                Then:   A 不能蓋房子
                """)]
    public async Task 已抵押的土地不能蓋房子()
    {
        // Arrange
        var a = new { Id = "A", Money = 1000m };
        var a2 = new { Id = "A2", Price = 1000m, House = 2, IsMortgage = true };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(a.Id)
            .WithMoney(a.Money)
            .WithPosition(a2.Id, Direction.Right)
            .WithLandContract(a2.Id, inMortgage: a2.IsMortgage)
            .Build()
        )
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(a.Id)
            .Build()
        )
        .WithLandHouse(a2.Id, a2.House);

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.Requests.PlayerBuildHouse();

        // Assert
        // A 蓋房子失敗
        hub.FluentAssert.PlayerCannotBuildHouseEvent(new PlayerCannotBuildHouseEventArgs
        {
            PlayerId = "A",
            LandId = "A2"
        });
    }
}
