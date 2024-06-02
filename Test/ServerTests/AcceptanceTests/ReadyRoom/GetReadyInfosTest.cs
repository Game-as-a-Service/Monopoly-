using Monopoly.DomainLayer.ReadyRoom.Builders;
using SharedLibrary.ResponseArgs.ReadyRoom.Models;
using LocationEnum = Monopoly.DomainLayer.ReadyRoom.Enums.LocationEnum;
using ReadyInfoLocationEnum = SharedLibrary.ResponseArgs.ReadyRoom.Models.LocationEnum;

namespace ServerTests.AcceptanceTests.ReadyRoom;

[TestClass]
public class GetReadyInfosTest : AbstractReadyRoomTestBase
{
    [TestMethod]
    [Description(
        """
        Given:  有兩間房間，其中一間有3名玩家在內
                - 房間1: 玩家1, 玩家2, 玩家3，房主為玩家3
                    - 玩家1: 角色 "Mei"，位置 First，準備
                    - 玩家2: 角色 "Dai"，無位置，未準備
                    - 玩家3: 無角色，位置 Third，未準備
                - 房間2: 無玩家
                
        When:   房間1的玩家2取得房間內的玩家準備資訊
        Then:   應該回傳玩家準備資訊
        """)]
    public async Task GetReadyRoomInfosOfConnection()
    {
        // Arrange
        var player1 = new PlayerBuilder()
            .WithRole("Mei")
            .WithLocation(LocationEnum.First)
            .WithReady()
            .Build();
        var player2 = new PlayerBuilder()
            .WithRole("Dai")
            .Build();
        var player3 = new PlayerBuilder()
            .WithLocation(LocationEnum.Third)
            .Build();
        var readyRoom = new ReadyRoomBuilder()
            .WithPlayer(player1)
            .WithPlayer(player2)
            .WithPlayer(player3)
            .WithHost(player3.Id)
            .Build();

        var expectedReadyInfos = new ReadyRoomInfos(
            player2.Id,
            [
                new Player(player1.Id, player1.Name, true, ReadyInfoLocationEnum.First, RoleEnum.Mei),
                new Player(player2.Id, player2.Name, false, ReadyInfoLocationEnum.None, RoleEnum.Dai),
                new Player(player3.Id, player3.Name, false, ReadyInfoLocationEnum.Third, RoleEnum.None)
            ],
            player3.Id
        );

        var readyRoom2 = new ReadyRoomBuilder().Build();

        await ReadyRoomRepository.SaveReadyRoomAsync(readyRoom);
        await ReadyRoomRepository.SaveReadyRoomAsync(readyRoom2);
        var hub = await Server.CreateReadyRoomHubConnectionAsync(readyRoom.Id, player2.Id);

        // Act
        var response = await hub.Requests.GetReadyRoomInfos();

        // Assert
        CollectionAssert.AreEquivalent(expectedReadyInfos.Players, response.Players);
        Assert.AreEqual(expectedReadyInfos.HostId, response.HostId);
    }
}