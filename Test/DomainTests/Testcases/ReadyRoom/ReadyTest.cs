using Monopoly.DomainLayer.ReadyRoom.Builders;
using Monopoly.DomainLayer.ReadyRoom.Enums;
using Monopoly.DomainLayer.ReadyRoom.Events;
using Monopoly.DomainLayer.ReadyRoom.Exceptions;

namespace DomainTests.Testcases.ReadyRoom;

[TestClass]

public class ReadyTest
{
    [TestMethod]
    [Description(
        """
        Given:  A: 位置1，角色1，未準備
        When:   A 按下準備
        Then:   A 的準備狀態:已準備
        """)]
    public void 玩家成功準備()
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
        
        var expectedDomainEvent = new PlayerReadyEvent(playerA.Id, ReadyStateEnum.Ready);

        // Act 
        readyRoom.PlayerReady(playerA.Id);

        // Assert
        readyRoom.DomainEvents.NextShouldBe(expectedDomainEvent).WithNoEvents();
    }

    [TestMethod]
    [Description(
        """
        Given:  A: 已準備
        When:   A 取消準備
        Then:   A 的準備狀態:未準備
        """)]
    public void 玩家取消準備()
    {
        // Arrange
        var playerA = new PlayerBuilder()
            .WithId("A")
            .WithReady()
            .Build();
        
        var readyRoom = new ReadyRoomBuilder()
            .WithPlayer(playerA)
            .Build();
        
        var expectedDomainEvent = new PlayerReadyEvent(playerA.Id, ReadyStateEnum.NotReady);
        
        // Act
        readyRoom.PlayerReady(playerA.Id);
        
        // Assert
        readyRoom.DomainEvents.NextShouldBe(expectedDomainEvent).WithNoEvents();
    }

    [TestMethod]
    [Description(
        """
        Given:  A: 未選擇位置, 未準備
        When:   A 按下準備
        Then:   擲出例外 PlayerLocationNotSetException
        """)]
    public void 玩家未選擇位置按下準備()
    {
        // Arrange
        var playerA = new PlayerBuilder()
            .WithId("A")
            .Build();
        var readyRoom = new ReadyRoomBuilder()
            .WithPlayer(playerA)
            .Build();
        
        // Act
        // Assert
        Assert.ThrowsException<PlayerLocationNotSetException>(() => readyRoom.PlayerReady(playerA.Id));
    }

    [TestMethod]
    [Description(
        """
        Given:  A: 位置1, 未準備
        When:   A 按下準備
        Then:   擲出例外 PlayerRoleNotSelectedException
        """)]
    public void 玩家未選擇角色按下準備()
    {
        // Arrange
        var playerA = new PlayerBuilder()
            .WithId("A")
            .WithLocation(LocationEnum.First)
            .Build();
        var readyRoom = new ReadyRoomBuilder()
            .WithPlayer(playerA)
            .Build();
        
        // Act
        // Assert
        Assert.ThrowsException<PlayerRoleNotSelectedException>(() => readyRoom.PlayerReady(playerA.Id));
    }
}