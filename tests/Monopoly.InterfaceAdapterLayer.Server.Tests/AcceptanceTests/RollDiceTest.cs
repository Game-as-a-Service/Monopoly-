using SharedLibrary.ResponseArgs.Monopoly;
using static Monopoly.InterfaceAdapterLayer.Server.Tests.Utils;

namespace Monopoly.InterfaceAdapterLayer.Server.Tests.AcceptanceTests;

[TestClass]
public class RollDiceTest
{
    private MonopolyTestServer _server = default!;

    [TestInitialize]
    public void Setup()
    {
        _server = new MonopolyTestServer();
    }

    [TestMethod]
    [Description(
        """
        Given:  目前玩家在F4
        When:   玩家擲骰得到6點
        Then:   A 移動到 A4
        """)]
    public async Task 玩家擲骰後移動棋子()
    {
        // Arrange
        var a = new { Id = "A" };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
            .WithPlayer(
                new PlayerBuilder(a.Id)
                    .WithPosition("F4", Direction.Up)
                    .Build()
            )
            .WithMockDice([6])
            .WithCurrentPlayer(new CurrentPlayerStateBuilder(a.Id).Build());

        monopolyBuilder.Save(_server);

        var hub = await _server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.Requests.PlayerRollDice();

        // Assert
        // A 擲了 6 點
        // A 移動到 Start，方向為 Right，剩下 5 步
        // A 移動到 A1，方向為 Right，剩下 4 步
        // A 移動到 Station1，方向為 Right，剩下 3 步
        // A 移動到 A2，方向為 Right，剩下 2 步
        // A 移動到 A3，方向為 Down，剩下 1 步
        // A 移動到 A4，方向為 Down，剩下 0 步
        hub.FluentAssert.PlayerRolledDiceEvent(new PlayerRolledDiceEventArgs
        {
            PlayerId = a.Id,
            DicePoints = [6]
        })
            .ThroughStartEvent(new PlayerThroughStartEventArgs
            {
                PlayerId = a.Id,
                GainMoney = 3000,
                TotalMoney = 18000
            });
        VerifyChessMovedEvent(hub, "A", "Start", "Right", 5);
        VerifyChessMovedEvent(hub, "A", "A1", "Right", 4);
        VerifyChessMovedEvent(hub, "A", "Station1", "Right", 3);
        VerifyChessMovedEvent(hub, "A", "A2", "Right", 2);
        VerifyChessMovedEvent(hub, "A", "A3", "Down", 1);
        VerifyChessMovedEvent(hub, "A", "A4", "Down", 0);
    }

    [TestMethod]
    [Description(
        """
        Given:  目前玩家在F4
        When:   玩家擲骰得到8點
        Then:   A 移動到 停車場
                玩家需要選擇方向
                玩家剩餘步數為 1
        """)]
    public async Task 玩家擲骰後移動棋子到需要選擇方向的地方()
    {
        // Arrange
        var a = new { Id = "A" };
        string[] expectedDirections = ["Right", "Down", "Left"];
        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
            .WithPlayer(
                new PlayerBuilder(a.Id)
                    .WithPosition("F4", Direction.Up)
                    .Build()
            )
            .WithMockDice([2, 6])
            .WithCurrentPlayer(new CurrentPlayerStateBuilder(a.Id).Build());

        monopolyBuilder.Save(_server);

        var hub = await _server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.Requests.PlayerRollDice();

        //Assert
        // A 擲了 8 點
        // A 移動到 Start，方向為 Right，剩下 7 步
        // A 移動到 A1，方向為 Right，剩下 6 步
        // A 移動到 Station1，方向為 Right，剩下 5 步
        // A 移動到 A2，方向為 Right，剩下 4 步
        // A 移動到 A3，方向為 Down，剩下 3 步
        // A 移動到 A4，方向為 Down，剩下 2 步
        // A 移動到 ParkingLot，方向為 Down，剩下 1 步
        // A 需要選擇方向，可選擇的方向為 Right, Down, Left
        hub.FluentAssert.PlayerRolledDiceEvent(new PlayerRolledDiceEventArgs
        {
            PlayerId = a.Id,
            DicePoints = [2, 6]
        });
        hub.FluentAssert.ThroughStartEvent(new PlayerThroughStartEventArgs
        {
            PlayerId = a.Id,
            GainMoney = 3000,
            TotalMoney = 18000
        });
        VerifyChessMovedEvent(hub, "A", "Start", "Right", 7);
        VerifyChessMovedEvent(hub, "A", "A1", "Right", 6);
        VerifyChessMovedEvent(hub, "A", "Station1", "Right", 5);
        VerifyChessMovedEvent(hub, "A", "A2", "Right", 4);
        VerifyChessMovedEvent(hub, "A", "A3", "Down", 3);
        VerifyChessMovedEvent(hub, "A", "A4", "Down", 2);
        //VerifyChessMovedEvent(hub, "A", "ParkingLot", "Down", 1);
        hub.FluentAssert.PlayerNeedToChooseDirectionEvent(new PlayerNeedToChooseDirectionEventArgs
        {
            PlayerId = a.Id,
            Directions = expectedDirections
        });
    }

    [TestMethod]
    [Description(
        """
        Given
            目前玩家A在F3
            玩家持有1000元
        When
            玩家擲骰得到4點
        Then
            玩家移動到 A1
            玩家剩餘步數為 0
            玩家持有4000元
        """)]
    public async Task 玩家擲骰後移動棋子經過起點獲得獎勵金3000()
    {
        // Arrange
        var a = new { Id = "A", Money = 1000m };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
            .WithPlayer(
                new PlayerBuilder(a.Id)
                    .WithMoney(a.Money)
                    .WithPosition("F3", Direction.Up)
                    .Build()
            )
            .WithMockDice([2, 2])
            .WithCurrentPlayer(new CurrentPlayerStateBuilder(a.Id).Build());

        monopolyBuilder.Save(_server);

        var hub = await _server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.Requests.PlayerRollDice();

        // Assert
        // A 擲了 4 點
        // A 移動到 Station4，方向為 Up，剩下 3 步
        // A 移動到 F4，方向為 Up，剩下 2 步
        // A 移動到 Start，方向為 Right，剩下 1 步
        // A 獲得獎勵金3000，共持有4000元
        // A 移動到 A1，方向為 Right，剩下 0 步
        hub.FluentAssert.PlayerRolledDiceEvent(new PlayerRolledDiceEventArgs
        {
            PlayerId = a.Id,
            DicePoints = [2, 2]
        });
        VerifyChessMovedEvent(hub, "A", "Station4", "Up", 3);
        VerifyChessMovedEvent(hub, "A", "F4", "Up", 2);
        hub.FluentAssert.ThroughStartEvent(new PlayerThroughStartEventArgs
        {
            PlayerId = a.Id,
            GainMoney = 3000,
            TotalMoney = 4000
        });
        VerifyChessMovedEvent(hub, "A", "Start", "Right", 1);
        VerifyChessMovedEvent(hub, "A", "A1", "Right", 0);
        hub.FluentAssert.PlayerCanBuyLandEvent(new PlayerCanBuyLandEventArgs
        {
            PlayerId = a.Id,
            LandId = "A1",
            Price = 1000
        });
    }

    [TestMethod]
    [Description(
        """
        Given:  目前玩家在F3
                玩家持有1000元
        When:   玩家擲骰得到3點
        Then:   玩家移動到 起點
                玩家剩餘步數為 0
                玩家持有1000元
        """)]
    public async Task 玩家擲骰後移動棋子到起點無法獲得獎勵金()
    {
        // Arrange
        var a = new { Id = "A", Money = 1000m };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
            .WithPlayer(
                new PlayerBuilder(a.Id)
                    .WithMoney(a.Money)
                    .WithPosition("F3", Direction.Up)
                    .Build()
            )
            .WithMockDice([2, 1])
            .WithCurrentPlayer(new CurrentPlayerStateBuilder(a.Id).Build());

        monopolyBuilder.Save(_server);

        var hub = await _server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.Requests.PlayerRollDice();

        // Assert
        // A 擲了 3 點
        // A 移動到 Station4，方向為 Up，剩下 2 步
        // A 移動到 F4，方向為 Up，剩下 1 步
        // A 移動到 Start，方向為 Right，剩下 0 步
        // A 沒有獲得獎勵金，共持有1000元
        hub.FluentAssert.PlayerRolledDiceEvent(new PlayerRolledDiceEventArgs
        {
            PlayerId = a.Id,
            DicePoints = [2, 1]
        });
        VerifyChessMovedEvent(hub, "A", "Station4", "Up", 2);
        VerifyChessMovedEvent(hub, "A", "F4", "Up", 1);
        VerifyChessMovedEvent(hub, "A", "Start", "Right", 0);
        hub.FluentAssert.CannotGetRewardBecauseStandOnStartEvent(new CannotGetRewardBecauseStandOnStartEventArgs
        {
            PlayerId = a.Id
        });
    }

    [TestMethod]
    [Description("""
                 Given:  目前玩家在A1
                         玩家持有A2
                         A2房子不足5間
                 When:   玩家擲骰得到2點
                 Then:   玩家移動到 A2
                         玩家剩餘步數為 0
                         提示可以蓋房子
                 """)]
    public async Task 玩家擲骰後移動棋子到自己擁有地()
    {
        // Arrange
        var a = new { Id = "A" };
        var a1 = new { Id = "A1" };
        var a2 = new { Id = "A2", House = 4 };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
            .WithPlayer(
                new PlayerBuilder(a.Id)
                    .WithPosition(a1.Id, Direction.Right)
                    .WithLandContract(a2.Id)
                    .Build()
            )
            .WithMockDice([1, 1])
            .WithCurrentPlayer(new CurrentPlayerStateBuilder(a.Id).Build())
            .WithLandHouse(a2.Id, a2.House);

        monopolyBuilder.Save(_server);

        var hub = await _server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.Requests.PlayerRollDice();

        // Assert
        // A 擲了 2 點
        // A 移動到 A2，方向為 Right，剩下 0 步
        // A 可以蓋房子
        hub.FluentAssert.PlayerRolledDiceEvent(new PlayerRolledDiceEventArgs
        {
            PlayerId = a.Id,
            DicePoints = [1, 1]
        });
        VerifyChessMovedEvent(hub, "A", "Station1", "Right", 1);
        VerifyChessMovedEvent(hub, "A", "A2", "Right", 0);
        hub.FluentAssert.PlayerCanBuildHouseEvent(new PlayerCanBuildHouseEventArgs
        {
            PlayerId = a.Id,
            LandId = a2.Id,
            HouseCount = 4,
            Price = 1000
        });
    }

    [TestMethod]
    [Description("""
                 Given
                     目前玩家A在A1
                     玩家B持有A2(無房子)
                 When
                     玩家擲骰得到2點
                 Then
                     玩家A移動到 A2
                     玩家剩餘步數為 0
                     提示需要付過路費
                 """)]
    public async Task 玩家擲骰後移動棋子到他人擁有地()
    {
        // Arrange
        var a = new { Id = "A" };
        var b = new { Id = "B" };
        var a2 = new { Id = "A2", House = 0 };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
            .WithPlayer(
                new PlayerBuilder(a.Id)
                    .WithPosition("A1", Direction.Right)
                    .Build()
            )
            .WithPlayer(
                new PlayerBuilder(b.Id)
                    .WithLandContract(a2.Id)
                    .Build()
            )
            .WithMockDice([2])
            .WithCurrentPlayer(new CurrentPlayerStateBuilder(a.Id).Build());

        monopolyBuilder.Save(_server);

        var hub = await _server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.Requests.PlayerRollDice();

        // Assert
        // A 擲了 2 點
        // A 移動到 A2，方向為 Right，剩下 0 步
        // A 需要支付過路費
        hub.FluentAssert.PlayerRolledDiceEvent(new PlayerRolledDiceEventArgs
        {
            PlayerId = a.Id,
            DicePoints = [2]
        });
        VerifyChessMovedEvent(hub, "A", "Station1", "Right", 1);
        VerifyChessMovedEvent(hub, "A", "A2", "Right", 0);
        hub.FluentAssert.PlayerNeedsToPayTollEvent(new PlayerNeedsToPayTollEventArgs
        {
            PlayerId = a.Id,
            OwnerId = b.Id,
            Toll = 50
        });
    }

    [TestMethod]
    [Description("""
                 Given:  目前玩家在A1
                 When:   玩家擲骰得到2點
                 Then:   玩家移動到 A2
                         玩家剩餘步數為 0
                         提示可以購買空地
                 """)]
    public async Task 玩家擲骰後移動棋子到空地()
    {
        // Arrange
        var a = new { Id = "A" };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
            .WithPlayer(
                new PlayerBuilder(a.Id)
                    .WithPosition("A1", Direction.Right)
                    .Build()
            )
            .WithMockDice([2])
            .WithCurrentPlayer(new CurrentPlayerStateBuilder(a.Id).Build());

        monopolyBuilder.Save(_server);

        var hub = await _server.CreateHubConnectionAsync(gameId, "A");
        // Act
        await hub.Requests.PlayerRollDice();

        // Assert
        // A 擲了 2 點
        // A 移動到 A2，方向為 Right，剩下 0 步
        // A 可以購買空地
        hub.FluentAssert.PlayerRolledDiceEvent(new PlayerRolledDiceEventArgs
        {
            PlayerId = a.Id,
            DicePoints = [2]
        });
        VerifyChessMovedEvent(hub, "A", "Station1", "Right", 1);
        VerifyChessMovedEvent(hub, "A", "A2", "Right", 0);
        hub.FluentAssert.PlayerCanBuyLandEvent(new PlayerCanBuyLandEventArgs
        {
            PlayerId = a.Id,
            LandId = "A2",
            Price = 1000
        });
    }

    [TestMethod]
    [Description("""
                 Given:  目前玩家在Station2
                 When:   玩家擲骰得到2點
                 Then:   玩家移動到 R2
                         玩家剩餘步數為 0
                 """)]
    public async Task 玩家擲骰後移動棋子到道路()
    {
        // Arrange
        var a = new { Id = "A" };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
            .WithPlayer(
                new PlayerBuilder(a.Id)
                    .WithPosition("Station2", Direction.Down)
                    .Build()
            )
            .WithMockDice([2])
            .WithCurrentPlayer(new CurrentPlayerStateBuilder(a.Id).Build());

        monopolyBuilder.Save(_server);

        var hub = await _server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.Requests.PlayerRollDice();

        // Assert
        // A 擲了 2 點
        // A 移動到 R2，方向為 Left，剩下 0 步
        // A 可以購買空地
        hub.FluentAssert.PlayerRolledDiceEvent(new PlayerRolledDiceEventArgs
        {
            PlayerId = a.Id,
            DicePoints = [2]
        });
        VerifyChessMovedEvent(hub, "A", "D1", "Down", 1);
        VerifyChessMovedEvent(hub, "A", "R2", "Left", 0);
    }
}