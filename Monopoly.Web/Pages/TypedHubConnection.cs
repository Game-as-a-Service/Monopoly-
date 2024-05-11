using SharedLibrary;
using SignalR.Client.Generator;

namespace Client.Pages;

[TypedHubClient(typeof(IMonopolyRequests), typeof(IMonopolyResponses))]
public partial class TypedHubConnection;

public abstract class Hub<T>;

public interface IMonopolyRequests
{
    public Task GetReadyInfo();
    public Task PlayerSelectLocation(int locationId);
}
