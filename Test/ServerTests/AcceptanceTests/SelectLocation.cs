using Application.DataModels;
using Server.Hubs;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;
using static ServerTests.Utils;

namespace ServerTests.AcceptanceTests;

[TestClass]
public class SelectLocation
{
    private MonopolyTestServer server = default!;
    private const string GameId = "1";

    [TestInitialize]
    public void Setup()
    {
        server = new MonopolyTestServer();
    }

    [TestMethod]
    [Description("""
        Given:  玩家A:位置未選擇
        When:   玩家A選擇位置1
        Then:   玩家A的位置為1
        """)]
    public async Task 玩家選擇沒人的位置()
    {
        //Arrange
        var a = new { Id = "A", locationId = 0, selectLocationId = 1 };

        var monopolyBuilder = new MonopolyBuilder("1")
            .WithGameStage(GameStage.Preparing)
            .WithPlayer(
                new PlayerBuilder("A")
                .WithLocation(a.locationId)
                .Build()
            );
        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(GameId, "A");

        //Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerSelectLocation), a.selectLocationId);

        //Assert
        hub.Verify(
            nameof(IMonopolyResponses.PlayerSelectLocationEvent),
            (PlayerSelectLocationEventArgs e) => e is { PlayerId: "A", LocationId: 1 }
            );
        hub.VerifyNoElseEvent();
    }

    [TestMethod]
    [Description("""
        Given:  玩家A:位置未選擇
                玩家B:位置1
        When:   玩家A選擇位置1
        Then:   玩家A的位置為未選擇
        """)]
    public async Task 玩家選擇有人的位置()
    {
        //Arrange
        var a = new { Id = "A", locationId = 0, selectLocationId = 1 };
        var b = new { Id = "B", locationId = 1 };

        var monopolyBuilder = new MonopolyBuilder("1")
            .WithGameStage(GameStage.Preparing)
            .WithPlayer(
                new PlayerBuilder(a.Id)
                .WithLocation(a.locationId)
                .Build()
            )
            .WithPlayer(
                new PlayerBuilder(b.Id)
                .WithLocation(b.locationId)
                .Build()
            );
        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(GameId, "A");

        //Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerSelectLocation), a.selectLocationId);

        //Assert
        hub.Verify(
            nameof(IMonopolyResponses.PlayCannotSelectLocationEvent),
            (PlayCannotSelectLocationEventArgs e) => e is { PlayerId: "A", LocationId: 0 }
            );
        hub.VerifyNoElseEvent();
    }

    [TestMethod]
    [Description("""
        Given:  玩家A:位置1
        When:   玩家A選擇位置2
        Then:   玩家A的位置為2
        """)]
    public async Task 有選位置的玩家更換到沒人的位置()
    {
        //Arrange
        var a = new { Id = "A", locationId = 1, selectLocationId = 2 };

        var monopolyBuilder = new MonopolyBuilder("1")
            .WithGameStage(GameStage.Preparing)
            .WithPlayer(
                new PlayerBuilder("A")
                .WithLocation(a.locationId)
                .Build()
            );
        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(GameId, "A");

        //Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerSelectLocation), a.selectLocationId);

        //Assert
        hub.Verify(
            nameof(IMonopolyResponses.PlayerSelectLocationEvent),
            (PlayerSelectLocationEventArgs e) => e is { PlayerId: "A", LocationId: 2 } 
            );
        hub.VerifyNoElseEvent();
    }
}
