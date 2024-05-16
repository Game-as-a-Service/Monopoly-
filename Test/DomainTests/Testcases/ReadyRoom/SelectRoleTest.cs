using Monopoly.DomainLayer.ReadyRoom.Builders;
using Monopoly.DomainLayer.ReadyRoom.Events;

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
}