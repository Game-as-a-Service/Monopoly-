using SharedLibrary;
using SignalR.Client.Generator;

namespace Client.Pages;

[TypedHubClient(typeof(IMonopolyRequests), typeof(IMonopolyResponses))]
public partial class TypedHubConnection;

public interface IMonopolyRequests
{
    public Task GetReadyInfo();
    public Task PlayerSelectLocation(int locationId);
    public Task PlayerSelectRole(string roleId);
}
