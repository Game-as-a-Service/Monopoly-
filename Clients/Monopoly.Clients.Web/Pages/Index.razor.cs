using Client.Options;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using SharedLibrary.ResponseArgs.ReadyRoom;

namespace Client.Pages;

public partial class Index
{
    [Parameter] public string GameId { get; set; } = null!;

    [Parameter, SupplyParameterFromQuery(Name = "token")]
    public string AccessToken { get; set; } = default!;
    [Inject] private IOptions<MonopolyApiOptions> BackendApiOptions { get; set; } = default!;
    private string UserId { get; set; } = string.Empty;
    private List<string> Messages { get; } = [];
    public bool IsGameStarted { get; set; } = false;
    private TypedHubConnection Connection { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await SetupHubConnectionAsync();
    }

    private async Task SetupHubConnectionAsync()
    {
        var baseUri = new Uri(BackendApiOptions.Value.BaseUrl);
        var url = new Uri(baseUri, $"/monopoly?gameid={GameId}");
        var client = new HubConnectionBuilder()
            .WithUrl(url, options =>
            {
                options.AccessTokenProvider = async () => await Task.FromResult(AccessToken);
            })
            .Build();
        Connection = new TypedHubConnection(client);
        Connection.WelcomeEventHandler += OnWelcomeEventHandler;
        client.Closed += OnConnectionClosed;
        try
        {
            await client.StartAsync();
            Messages.Add("連線成功!");
        }
        catch (Exception ex)
        {
            Messages.Add(ex.Message);
        }
    }

    private async Task OnConnectionClosed(Exception? exception)
    {
        var errorMessage = exception?.Message;
        Messages.Add($"中斷連線: {errorMessage}");

        await Task.CompletedTask;
    }

    private Task OnWelcomeEventHandler(WelcomeEventArgs e)
    {
        UserId = e.PlayerId;
        Messages.Add($"歡迎 {e.PlayerId} 加入遊戲!");
        StateHasChanged();
        return Task.CompletedTask;
    }
}