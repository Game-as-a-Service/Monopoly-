using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Monopoly.InterfaceAdapterLayer.Server.Tests.AcceptanceTests.ReadyRoom;

[TestClass]
public class CreateReadyRoomTest : AbstractReadyRoomTestBase
{
    [TestMethod]
    [Description(
        """
        Given:  沒有房間
                User A 的 token
        When:   發送    
                POST /dev/create-room
                Authorization: Bearer {token}
        Then:   回傳房間 ID
                房間 ID 的房間已經被建立
        """)]
    public async Task Dev_創建房間()
    {
        // Arrange
        var token = await CreateUser("UserA");
        var request = new HttpRequestMessage(HttpMethod.Post, "/dev/room");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var client = Server.CreateClient();
        
        // Act
        var response = await client.SendAsync(request);
        
        // Assert
        response.EnsureSuccessStatusCode();
        var urlOfRoom = await response.Content.ReadFromJsonAsync<string>();
        
        Assert.IsNotNull(urlOfRoom);
        
        var roomId = urlOfRoom.Split('/').Last();
        
        var readyRoom = await ReadyRoomRepository.FindByIdAsync(roomId);
        Assert.IsNotNull(readyRoom);
    } 
}