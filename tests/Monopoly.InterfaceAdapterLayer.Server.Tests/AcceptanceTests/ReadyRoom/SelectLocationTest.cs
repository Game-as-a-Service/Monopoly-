using Microsoft.AspNetCore.SignalR;
using Monopoly.DomainLayer.ReadyRoom.Builders;
using Monopoly.DomainLayer.ReadyRoom.Enums;
using SharedLibrary.ResponseArgs.ReadyRoom;

namespace Monopoly.InterfaceAdapterLayer.Server.Tests.AcceptanceTests.ReadyRoom;

[TestClass]
public class SelectLocationTest : AbstractReadyRoomTestBase
{
    [TestMethod]
    [Description("""
                 Given:  玩家A:位置未選擇
                 When:   玩家A選擇位置 1
                 Then:   玩家A的位置為 1
                 """)]
    public async Task 玩家選擇沒人的位置()
    {
        // Arrange
        var playerA = new PlayerBuilder()
            .WithId("A")
            .Build();
        var readyRoom = new ReadyRoomBuilder()
            .WithPlayer(playerA)
            .Build();
        const LocationEnum location = LocationEnum.First;

        await ReadyRoomRepository.SaveAsync(readyRoom);
        var hub = await Server.CreateReadyRoomHubConnectionAsync(readyRoom.Id, playerA.Id);

        // Act
        await hub.Requests.SelectLocation((int)location);

        // Assert
        hub.FluentAssert.PlayerSelectLocationEvent(new PlayerSelectLocationEventArgs(playerA.Id, (int)location));
    }

    [TestMethod]
    [Description("""
                 Given:  玩家A:位置未選擇
                         玩家B:位置1
                 When:   玩家A選擇位置1
                 Then:   擲出例外 PlayerCannotSelectLocationDueToOccupiedException
                 """)]
    public async Task 玩家選擇有人的位置()
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
        await ReadyRoomRepository.SaveAsync(readyRoom);
        var hub = await Server.CreateReadyRoomHubConnectionAsync(readyRoom.Id, playerA.Id);

        // Act & Assert
        await Assert.ThrowsExceptionAsync<HubException>(async () => await hub.Requests.SelectLocation((int)location));
    }

    [TestMethod]
    [Description("""
                 Given:  玩家A:位置1
                 When:   玩家A選擇位置2
                 Then:   玩家A的位置為2
                 """)]
    public async Task 有選位置的玩家更換位置()
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
        await ReadyRoomRepository.SaveAsync(readyRoom);
        var hub = await Server.CreateReadyRoomHubConnectionAsync(readyRoom.Id, playerA.Id);

        // Act
        await hub.Requests.SelectLocation((int)location);

        // Assert
        hub.FluentAssert.PlayerSelectLocationEvent(new PlayerSelectLocationEventArgs(playerA.Id, (int)location));
    }

    [TestMethod]
    [Description("""
                 Given:  玩家A:位置1，已準備
                         位置2 沒有玩家
                 When:   玩家A選擇位置2
                 Then:   擲出錯誤 PlayerCannotSelectLocationDueToAlreadyReadyException
                 """)]
    public async Task 玩家已準備不能選擇位置()
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
        const LocationEnum location = LocationEnum.Second;
        var hub = await Server.CreateReadyRoomHubConnectionAsync(readyRoom.Id, playerA.Id);

        // Act & Assert
        await Assert.ThrowsExceptionAsync<HubException>(async () => await hub.Requests.SelectLocation((int)location));
    }
}