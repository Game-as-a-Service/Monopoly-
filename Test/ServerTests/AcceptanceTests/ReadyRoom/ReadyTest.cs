using Microsoft.AspNetCore.SignalR;
using Monopoly.DomainLayer.ReadyRoom.Builders;
using Monopoly.DomainLayer.ReadyRoom.Enums;
using SharedLibrary.ResponseArgs.ReadyRoom;

namespace ServerTests.AcceptanceTests.ReadyRoom;

[TestClass]
public class ReadyTest : AbstractReadyRoomTestBase
{
    [TestMethod]
    [Description(
        """
        Given:  A: 位置1，角色1，未準備
        When:   A 按下準備
        Then:   A 的準備狀態:已準備
        """)]
    public async Task 玩家成功準備()
    {
        // Arrange
        var playerA = new PlayerBuilder()
            .WithId("A")
            .WithLocation(LocationEnum.First)
            .WithRole("1")
            .Build();
        var readyRoom = new ReadyRoomBuilder()
            .WithPlayer(playerA)
            .Build();

        await ReadyRoomRepository.SaveReadyRoomAsync(readyRoom);
        var hub = await Server.CreateReadyRoomHubConnectionAsync(readyRoom.Id, playerA.Id);

        // Act
        await hub.Requests.PlayerReady();

        // Assert
        hub.FluentAssert.PlayerReadyEvent(new PlayerReadyEventArgs(playerA.Id, true));
    }

    [TestMethod]
    [Description(
        """
        Given:  A: 已準備
        When:   A 取消準備
        Then:   A 的準備狀態:未準備
        """)]
    public async Task 玩家取消準備()
    {
        // Arrange
        var playerA = new PlayerBuilder()
            .WithId("A")
            .WithReady()
            .Build();

        var readyRoom = new ReadyRoomBuilder()
            .WithPlayer(playerA)
            .Build();

        await ReadyRoomRepository.SaveReadyRoomAsync(readyRoom);
        var hub = await Server.CreateReadyRoomHubConnectionAsync(readyRoom.Id, playerA.Id);

        // Act
        await hub.Requests.PlayerReady();

        // Assert
        hub.FluentAssert.PlayerReadyEvent(new PlayerReadyEventArgs(playerA.Id, false));
    }

    [TestMethod]
    [Description(
        """
        Given:  A: 未選擇位置, 未準備
        When:   A 按下準備
        Then:   擲出例外 PlayerLocationNotSetException
        """)]
    public async Task 玩家未選擇位置按下準備()
    {
        // Arrange
        var playerA = new PlayerBuilder()
            .WithId("A")
            .Build();
        var readyRoom = new ReadyRoomBuilder()
            .WithPlayer(playerA)
            .Build();

        await ReadyRoomRepository.SaveReadyRoomAsync(readyRoom);
        var hub = await Server.CreateReadyRoomHubConnectionAsync(readyRoom.Id, playerA.Id);

        // Act & Assert
        await Assert.ThrowsExceptionAsync<HubException>(() => hub.Requests.PlayerReady());
    }

    [TestMethod]
    [Description(
        """
        Given:  A: 位置1, 未準備
        When:   A 按下準備
        Then:   擲出例外 PlayerRoleNotSelectedException
        """)]
    public async Task 玩家未選擇角色按下準備()
    {
        // Arrange
        var playerA = new PlayerBuilder()
            .WithId("A")
            .WithLocation(LocationEnum.First)
            .Build();
        var readyRoom = new ReadyRoomBuilder()
            .WithPlayer(playerA)
            .Build();

        await ReadyRoomRepository.SaveReadyRoomAsync(readyRoom);
        var hub = await Server.CreateReadyRoomHubConnectionAsync(readyRoom.Id, playerA.Id);

        // Act & Assert
        await Assert.ThrowsExceptionAsync<HubException>(() => hub.Requests.PlayerReady());
    }
}