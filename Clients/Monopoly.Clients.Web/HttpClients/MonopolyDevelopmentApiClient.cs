using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Client.HttpClients;

public class MonopolyDevelopmentApiClient(HttpClient httpClient)
{
    public async Task<JwtSecurityToken> CreateUserAsync(string userName)
    {
        var response = await httpClient.PostAsync($"/dev/user?userName={userName}", null);
        response.EnsureSuccessStatusCode();
        var token = await response.Content.ReadFromJsonAsync<string>();
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        return jwtToken;
    }

    public async Task<string> CreateRoomAsync(JwtSecurityToken hostToken, string[] playerIds)
    {
        var payload = new
        {
            PlayerIds = playerIds
        };
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", hostToken.RawData);
        var response = await httpClient.PostAsJsonAsync("/dev/room", payload);
        response.EnsureSuccessStatusCode();
        var roomId = await response.Content.ReadFromJsonAsync<string>();
        return roomId!;
    }
}

public static class JwtSecurityTokenExtensions
{
    public static string GetMonopolyPlayerId(this JwtSecurityToken token)
    {
        var payload = token.Payload;
        return payload.Claims.FirstOrDefault(x => x.Type == "nameid")?.Value ?? "";
    }
}