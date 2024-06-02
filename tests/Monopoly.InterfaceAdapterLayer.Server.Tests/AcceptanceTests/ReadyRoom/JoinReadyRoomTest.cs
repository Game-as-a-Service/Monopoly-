using Microsoft.AspNetCore.SignalR;
using Monopoly.DomainLayer.ReadyRoom.Builders;

namespace Monopoly.InterfaceAdapterLayer.Server.Tests.AcceptanceTests.ReadyRoom;

[TestClass]
public class JoinReadyRoomTest : AbstractReadyRoomTestBase
{
    [TestMethod]
    [Description(
        """
        Given:  玩家 A 開了一個房間，裡面沒有任何玩家
                玩家 B 沒有加入房間
        When:   玩家 B 加入房間
        Then:   玩家 B 成功加入房間
        """)]
    public async Task 連接時不是房間中的玩家可以加入房間()
    {
        // Arrange
        var readyRoom = new ReadyRoomBuilder()
            .Build();
        
        await ReadyRoomRepository.SaveReadyRoomAsync(readyRoom);
        
        var hub = await Server.CreateReadyRoomHubConnectionAsync(readyRoom.Id, "PlayerB");
        
        // Act
        await hub.Requests.JoinRoom();
        
        // Assert
        var readyRoomAfterPlayerJoinRoom = await ReadyRoomRepository.GetReadyRoomAsync(readyRoom.Id);
        Assert.AreEqual(1, readyRoomAfterPlayerJoinRoom.Players.Count);
        Assert.AreEqual("PlayerB", readyRoomAfterPlayerJoinRoom.Players[0].Id);
    }
    
    [TestMethod]
    [Description(
        """
        Given:  玩家 A 開了一個房間，裡面有玩家 B
                玩家 B 已經加入房間
        When:   玩家 B 再次加入房間
        Then:   玩家 B 不能再次加入房間
        """)]
    public async Task 連接時是房間中的玩家無法加入房間()
    {
        // Arrange
        var playerB = new PlayerBuilder().Build();
        var readyRoom = new ReadyRoomBuilder()
            .WithPlayer(playerB)
            .Build();
        
        await ReadyRoomRepository.SaveReadyRoomAsync(readyRoom);
        
        var hub = await Server.CreateReadyRoomHubConnectionAsync(readyRoom.Id, playerB.Id);
        
        // Act & Assert
        await Assert.ThrowsExceptionAsync<HubException>(() => hub.Requests.JoinRoom());
    }
}