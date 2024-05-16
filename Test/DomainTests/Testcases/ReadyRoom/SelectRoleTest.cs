using Monopoly.DomainLayer.ReadyRoom.Builders;
using Monopoly.DomainLayer.ReadyRoom.Events;
using Monopoly.DomainLayer.ReadyRoom.Exceptions;

namespace DomainTests.Testcases.ReadyRoom;

[TestClass]
public class SelectRoleTest
{
    [TestMethod]
    [Description("""
                 Given:  玩家A在房間內，此時無人物
                 When:   玩家A選擇角色Id為 "1"
                 Then:   玩家A的角色為Id "1"
                 """)]
    public void 玩家選擇角色()
    {
        // Arrange
        var playerA = new PlayerBuilder()
            .WithId("A")
            .Build();
        var readyRoom = new ReadyRoomBuilder()
            .WithPlayer(playerA)
            .Build();
        const string roleId = "1";
        var expectedDomainEvent = new PlayerRoleSelectedEvent(playerA.Id, roleId);

        // Act
        readyRoom.SelectRole(playerA.Id, roleId);

        // Assert
        readyRoom.DomainEvents.NextShouldBe(expectedDomainEvent).WithNoEvents();
    }

    [TestMethod]
    [Description("""
                 Given:  玩家A在房間內，已經準備好並選擇了角色
                 When:   玩家A再次嘗試選擇角色
                 Then:   應該拋出異常 PlayerCannotSelectRoleDueToAlreadyReadyException
                 """)]
    public void 玩家已準備時不能選擇角色()
    {
        // Arrange
        var playerA = new PlayerBuilder()
            .WithId("A")
            .WithRole("1")
            .WithReady()
            .Build();
        var readyRoom = new ReadyRoomBuilder()
            .WithPlayer(playerA)
            .Build();
        const string newRoleId = "2";

        // Act & Assert
        Assert.ThrowsException<PlayerCannotSelectRoleDueToAlreadyReadyException>(() => readyRoom.SelectRole(playerA.Id, newRoleId));
    }
}