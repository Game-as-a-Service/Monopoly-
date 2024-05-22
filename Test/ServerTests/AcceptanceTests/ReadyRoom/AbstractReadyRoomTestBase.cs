using Application.Usecases.ReadyRoom;

namespace ServerTests.AcceptanceTests.ReadyRoom;

public abstract class AbstractReadyRoomTestBase
{
    private protected MonopolyTestServer Server = default!;
    private protected IReadyRoomRepository ReadyRoomRepository = default!;

    [TestInitialize]
    public void Setup()
    {
        Server = new MonopolyTestServer();
        ReadyRoomRepository = Server.GetRequiredService<IReadyRoomRepository>();
    }

    [TestCleanup]
    public void Cleanup()
    {
        Server.Dispose();
    }
}