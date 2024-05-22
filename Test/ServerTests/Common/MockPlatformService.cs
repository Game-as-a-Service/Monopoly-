using System.IdentityModel.Tokens.Jwt;
using Server.DataModels;
using Server.Services;

namespace ServerTests.Common;

public class MockPlatformService : IPlatformService
{
    public Task<UserInfo> GetUserInfo(string tokenString)
    {
        var jwt = new JwtSecurityToken(tokenString);

        var id = jwt.Claims.First(x => x.Type == "Id").Value;

        var userinfo = new UserInfo(id, "", "");

        return Task.FromResult(userinfo);
    }
}