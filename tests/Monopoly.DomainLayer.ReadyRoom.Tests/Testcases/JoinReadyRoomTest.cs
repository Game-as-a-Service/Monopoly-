using Monopoly.DomainLayer.ReadyRoom.Builders;
using Monopoly.DomainLayer.ReadyRoom.Events;
using Monopoly.DomainLayer.ReadyRoom.Exceptions;

namespace Monopoly.DomainLayer.ReadyRoom.Tests.Testcases;

[TestClass]
public class JoinReadyRoomTest
{
    [TestMethod]
    [Description(
        """
        Given:  房間0~3位玩家
        When:   玩家A加入房間
        Then:   玩家A已加入房間
        """)]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    public void 玩家加入房間(int playerCount)
    {
        // Arrange
        var players = Enumerable.Range(0, playerCount)
            .Select(i => new PlayerBuilder().WithId(i.ToString()).Build())
            .ToList();
        var readyRoomBuilder = new ReadyRoomBuilder();
        players.ForEach(player => readyRoomBuilder.WithPlayer(player));

        var readyRoom = readyRoomBuilder.Build();

        // Act
        readyRoom.PlayerJoin("A");

        // Assert
        readyRoom.DomainEvents.NextShouldBe(new PlayerJoinReadyRoomEvent("A")).WithNoEvents();
    }

    [TestMethod]
    [Description(
        """
        Given:  房間已滿四人
        When:   玩家A加入房間
        Then:   丟出房間已滿例外
        """)]
    public void 房間已滿四人無法加入房間()
    {
        // Arrange
        var players = Enumerable.Range(0, 4)
            .Select(i => new PlayerBuilder().WithId(i.ToString()).Build())
            .ToList();
        var readyRoomBuilder = new ReadyRoomBuilder();
        players.ForEach(player => readyRoomBuilder.WithPlayer(player));

        var readyRoom = readyRoomBuilder.Build();

        // Act & Assert
        Assert.ThrowsException<CannotJoinReadyRoomDueToRoomIsFullException>(() => readyRoom.PlayerJoin("A"));
    }

    [TestMethod]
    [Description(
        """
        Given:  玩家A已在房間內
        When:   玩家A加入房間
        Then:   丟出玩家已在房間內例外
        """)]
    public void 當玩家已經在房間內時無法加入房間()
    {
        // Arrange
        var readyRoom = new ReadyRoomBuilder()
            .WithPlayer(new PlayerBuilder().WithId("A").Build())
            .Build();

        // Act & Assert
        Assert.ThrowsException<PlayerAlreadyInRoomException>(() => readyRoom.PlayerJoin("A"));
    }
}