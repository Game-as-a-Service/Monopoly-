using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.DomainLayer.Domain;
using Monopoly.DomainLayer.ReadyRoom.Builders;
using Monopoly.DomainLayer.ReadyRoom.Common;
using Moq;
using SharedLibrary.ResponseArgs.ReadyRoom;

namespace Monopoly.InterfaceAdapterLayer.Server.Tests.AcceptanceTests.ReadyRoom;

[TestClass]
public class GameStartTest : AbstractReadyRoomTestBase
{
    [TestMethod]
    [Description(
        """
        Given:  房內有4名玩家，除了玩家A以外的所有玩家都已經準備
        When:   房主按下開始遊戲
        Then:   遊戲開始
                repository 的遊戲存在
        """)]
    public async Task 房主成功開始遊戲()
    {
        // Arrange
        const string gameId = "gameId";
        var gameIdProvider = new Mock<IGameIdProvider>();
        gameIdProvider.Setup(x => x.GetGameId()).Returns(gameId);

        var playerA = new PlayerBuilder()
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

        await ReadyRoomRepository.SaveAsync(readyRoom);

        var hub = await Server.CreateReadyRoomHubConnectionAsync(readyRoom.Id, playerA.Id);

        // Act
        await hub.Requests.StartGame();

        // Assert
        hub.FluentAssert.GameStartedEvent(new GameStartedEventArgs(gameId));
        
        var gameRepository = Server.Services.GetRequiredService<IRepository<MonopolyAggregate>>();
        var game = gameRepository.FindByIdAsync(gameId);
    }

    [TestMethod]
    [Description(
        """
        Given:  房內有4名玩家，但有玩家未準備
        When:   房主按下開始遊戲
        Then:   房主無法成功開始遊戲
        """)]
    public async Task 房主無法成功開始遊戲()
    {
        // Arrange
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
            .Build();

        var readyRoom = new ReadyRoomBuilder()
            .WithPlayer(playerA)
            .WithPlayer(playerB)
            .WithPlayer(playerC)
            .WithPlayer(playerD)
            .WithHost(playerA.Id)
            .Build();

        await ReadyRoomRepository.SaveAsync(readyRoom);
        var hub = await Server.CreateReadyRoomHubConnectionAsync(readyRoom.Id, playerA.Id);

        // Act & Assert
        await Assert.ThrowsExceptionAsync<HubException>(() => hub.Requests.StartGame());
    }


    [TestMethod]
    [Description(
        """
        Given:  房內有4名玩家，每個玩家都已經準備
        When:   非房主按下開始遊戲
        Then:   非房主無法成功開始遊戲
        """)]
    public async Task 非房主無法成功開始遊戲()
    {
        // Arrange
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
            .Build();

        await ReadyRoomRepository.SaveAsync(readyRoom);
        var hub = await Server.CreateReadyRoomHubConnectionAsync(readyRoom.Id, playerB.Id);

        // Act & Assert
        await Assert.ThrowsExceptionAsync<HubException>(() => hub.Requests.StartGame());
    }
}