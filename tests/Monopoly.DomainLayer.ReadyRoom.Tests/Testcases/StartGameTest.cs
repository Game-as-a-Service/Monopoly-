using Monopoly.DomainLayer.ReadyRoom.Builders;
using Monopoly.DomainLayer.ReadyRoom.Events;
using Moq;

namespace Monopoly.DomainLayer.ReadyRoom.Tests.Testcases;

[TestClass]
public class StartGameTest
{
    [TestMethod]
    [Description(
        """
        Given:  房內有4名玩家，每個玩家都已經準備
        When:   房主按下開始遊戲
        Then:   遊戲開始
        """)]
    public void 房主成功開始遊戲()
    {
        // Arrange
        const string gameId = "gameId";
        var gameIdProvider = new Mock<IGameIdProvider>();
        gameIdProvider.Setup(x => x.GetGameId()).Returns(gameId);
        
        var playerA = new PlayerBuilder()
            .WithReady()
            .Build();

        var playerB = new PlayerBuilder()
            .WithReady()
            .Build();

        var playerC = new PlayerBuilder()
            .WithReady()
            .Build();

        var playerD = new PlayerBuilder()
            .WithReady()
            .Build();

        var readyRoom = new ReadyRoomBuilder()
            .WithPlayer(playerA)
            .WithPlayer(playerB)
            .WithPlayer(playerC)
            .WithPlayer(playerD)
            .WithHost(playerA.Id)
            .WithGameIdProvider(gameIdProvider.Object)
            .Build();

        // Act
        readyRoom.StartGame(playerA.Id);

        // Assert
        readyRoom.DomainEvents.NextShouldBe(new GameStartedEvent(gameId)).WithNoEvents();
    }

    [TestMethod]
    public void 房主無法成功開始遊戲()
    {
    }
}