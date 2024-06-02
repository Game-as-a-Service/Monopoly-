using Microsoft.AspNetCore.SignalR;
using Monopoly.DomainLayer.ReadyRoom.Builders;

namespace Monopoly.InterfaceAdapterLayer.Server.Tests.AcceptanceTests.ReadyRoom;

[TestClass]
public class SelectRoleTest : AbstractReadyRoomTestBase
{
    [TestMethod]
    [Description("""
                 Given:  玩家A在房間內，此時無人物
                 When:   玩家A選擇角色Id為 "1"
                 Then:   玩家A的角色為Id "1"
                 """)]
    public async Task 玩家選擇角色()
    {
        // Arrange
        var playerA = new PlayerBuilder()
            .WithId("A")
            .Build();
        var readyRoom = new ReadyRoomBuilder()
            .WithPlayer(playerA)
            .Build();

        const string roleId = "1";

        await ReadyRoomRepository.SaveReadyRoomAsync(readyRoom);
        var hub = await Server.CreateReadyRoomHubConnectionAsync(readyRoom.Id, playerA.Id);

        // Act
        await hub.Requests.SelectRole(roleId);
    }

    [TestMethod]
    [Description("""
                 Given:  玩家A在房間內，已經準備好並選擇了角色
                 When:   玩家A再次嘗試選擇角色
                 Then:   應該拋出異常 PlayerCannotSelectRoleDueToAlreadyReadyException
                 """)]
    public async Task 玩家已準備時不能選擇角色()
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

        await ReadyRoomRepository.SaveReadyRoomAsync(readyRoom);
        var hub = await Server.CreateReadyRoomHubConnectionAsync(readyRoom.Id, playerA.Id);

        // Act & Assert
        await Assert.ThrowsExceptionAsync<HubException>(() => hub.Requests.SelectRole(newRoleId));
    }
}