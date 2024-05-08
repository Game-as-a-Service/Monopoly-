using SharedLibrary;
using SignalR.Client.Generator;

namespace Client.Pages;

[TypedHubClient(typeof(MonopolyHub))]
public partial class TypedHubConnection;

public abstract class Hub<T>;

public abstract class MonopolyHub : Hub<IMonopolyResponses>
{
    public abstract Task GetReadyInfo();
    public abstract Task PlayerSelectLocation(int locationId);
}
