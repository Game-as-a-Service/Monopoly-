using Monopoly.DomainLayer.ReadyRoom.Builders;
using Monopoly.DomainLayer.ReadyRoom.Enums;
using Monopoly.DomainLayer.ReadyRoom.Events;
using Monopoly.DomainLayer.ReadyRoom.Exceptions;

namespace DomainTests.Testcases.ReadyRoom;

[TestClass]
public class SelectLocationTest
{
    [TestMethod]
    [Description("""
                 Given:  玩家A:位置未選擇
                 When:   玩家A選擇位置 1
                 Then:   玩家A的位置為 1
                 """)]
    public void 玩家選擇沒人的位置()
    {
        // Arrange
        var playerA = new PlayerBuilder()
            .WithId("A")
            .Build();
        var readyRoom = new ReadyRoomBuilder()
            .WithPlayer(playerA)
            .Build();
        const LocationEnum location = LocationEnum.First;
        
        var expectedDomainEvent = new PlayerLocationSelectedEvent(playerA.Id, location);

        // Act
        readyRoom.SelectLocation(playerA.Id, location);

        // Assert
        readyRoom.DomainEvents.NextShouldBe(expectedDomainEvent).WithNoEvents();
    }

    [TestMethod]
    [Description("""
                 Given:  玩家A:位置未選擇
                         玩家B:位置1
                 When:   玩家A選擇位置1
                 Then:   擲出例外 PlayerCannotSelectLocationDueToOccupiedException
                 """)]
    public void 玩家選擇有人的位置()
    {
        // Arrange
        var playerA = new PlayerBuilder()
            .WithId("A")
            .Build();
        const LocationEnum location = LocationEnum.First;
        var playerB = new PlayerBuilder()
            .WithId("B")
            .WithLocation(location)
            .Build();
        var readyRoom = new ReadyRoomBuilder()
            .WithPlayer(playerA)
            .WithPlayer(playerB)
            .Build();

        // Act & Assert
        Assert.ThrowsException<PlayerCannotSelectLocationDueToOccupiedException>(() =>
            readyRoom.SelectLocation(playerA.Id, location));
    }

    [TestMethod]
    [Description("""
                 Given:  玩家A:位置1
                 When:   玩家A選擇位置2
                 Then:   玩家A的位置為2
                 """)]
    public void 有選位置的玩家更換位置()
    {
        // Arrange
        var playerA = new PlayerBuilder()
            .WithId("A")
            .WithLocation(LocationEnum.First)
            .Build();
        var readyRoom = new ReadyRoomBuilder()
            .WithPlayer(playerA)
            .Build();
        const LocationEnum location = LocationEnum.Second;
        var expectedDomainEvent = new PlayerLocationSelectedEvent(playerA.Id, location);

        // Act
        readyRoom.SelectLocation(playerA.Id, location);

        // Assert
        readyRoom.DomainEvents.NextShouldBe(expectedDomainEvent).WithNoEvents();
    }

    [TestMethod]
    [Description("""
                 Given:  玩家A:位置1，已準備
                         位置2 沒有玩家
                 When:   玩家A選擇位置2
                 Then:   擲出錯誤 PlayerCannotSelectLocationDueToAlreadyReadyException
                 """)]
    public void 玩家已準備不能選擇位置()
    {
        // Arrange
        var playerA = new PlayerBuilder()
            .WithId("A")
            .WithLocation(LocationEnum.First)
            .WithReady()
            .Build();
        var readyRoom = new ReadyRoomBuilder()
            .WithPlayer(playerA)
            .Build();

        // Act & Assert
        Assert.ThrowsException<PlayerCannotSelectLocationDueToAlreadyReadyException>(() =>
            readyRoom.SelectLocation(playerA.Id, LocationEnum.Second));
    }
}