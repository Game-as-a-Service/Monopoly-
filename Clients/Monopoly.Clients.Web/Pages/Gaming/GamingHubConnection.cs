using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly.Models;
using SignalR.Client.Generator;

namespace Client.Pages.Gaming;

[TypedHubClient(typeof(IGamingRequests), typeof(IMonopolyResponses))]
public partial class GamingHubConnection;

public interface IGamingRequests
{
    Task PlayerRollDice();
    Task<MonopolyInfos> GetMonopolyInfos();
}