using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.InterfaceAdapterLayer.Server.Hubs.Monopoly;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;
using static Monopoly.InterfaceAdapterLayer.Server.Tests.Utils;

namespace Monopoly.InterfaceAdapterLayer.Server.Tests.AcceptanceTests;

[TestClass]
public class SelectDirectionTest
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
        Given:  目前玩家在監獄
                玩家方向為Down
                玩家剩餘步數為 0
                玩家目前需要選擇方向
        When:   玩家選擇方向為Left
        Then:   玩家停在 監獄
                玩家剩餘步數為 0
                玩家下一回合無法行動
        """)]
    public async Task 玩家選擇方向後在監獄停住()
    {
        // Arrange
        const string gameId = "1";
        var a = new { Id = "A", Money = 1000m };
        var aBlock = new { Id = "Jail" };

        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(a.Id)
            .WithMoney(a.Money)
            .WithPosition(aBlock.Id, Direction.Down)
            .Build()
        )
        .WithMockDice([2])
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(a.Id).Build());

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, a.Id);

        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerChooseDirection), Direction.Left.ToString());

        // Assert
        // A 選擇方向為 Left
        // A 停在 Jail，方向為 Left，剩下 0 步
        // A 下一回合無法行動，暫停2回合
        hub.Verify(nameof(IMonopolyResponses.PlayerChooseDirectionEvent),
            (PlayerChooseDirectionEventArgs e) => e is { PlayerId: "A", Direction: "Left" });
        hub.Verify(nameof(IMonopolyResponses.SuspendRoundEvent),
                (SuspendRoundEventArgs e) => e is { PlayerId: "A", SuspendRounds: 2 });
        hub.VerifyNoElseEvent();

        var repo = server.GetRequiredService<IRepository>();
        var game = repo.FindGameById("1");
        var player = game.Players.First(p => p.Id == a.Id);
        Assert.AreEqual("Jail", player.Chess.CurrentBlockId);
    }

    [TestMethod]
    [Description(
    """
        Given:  目前玩家在停車場
                玩家方向為Down
                玩家剩餘步數為 0
                玩家目前需要選擇方向
        When:   玩家選擇方向為Left
        Then:   玩家停在 停車場
                玩家剩餘步數為 0
        """)]
    public async Task 玩家選擇方向後在停車場停住()
    {
        // Arrange
        var a = new { Id = "A" };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(a.Id)
            .WithPosition("ParkingLot", Direction.Down)
            .Build()
        )
        .WithMockDice([2])
        .WithCurrentPlayer(
            new CurrentPlayerStateBuilder(a.Id)
                .WithRemainingSteps(0)
                .HadNotSelectedDirection().Build()
            );

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, a.Id);

        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerChooseDirection), Direction.Left.ToString());

        // Assert
        // A 選擇方向為 Left
        // A 停在 ParkingLot，方向為 Left，剩下 0 步

        hub.Verify(nameof(IMonopolyResponses.PlayerChooseDirectionEvent),
            (PlayerChooseDirectionEventArgs e) => e.PlayerId == a.Id && e.Direction == Direction.Left.ToString());

        hub.VerifyNoElseEvent();

        var repo = server.GetRequiredService<IRepository>();
        var game = repo.FindGameById("1");
        var player = game.Players.First(p => p.Id == a.Id);
        Assert.AreEqual("ParkingLot", player.Chess.CurrentBlockId);
    }
}