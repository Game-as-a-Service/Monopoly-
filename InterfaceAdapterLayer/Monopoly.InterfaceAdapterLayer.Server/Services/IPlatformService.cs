using Monopoly.InterfaceAdapterLayer.Server.DataModels;

namespace Monopoly.InterfaceAdapterLayer.Server.Services;

public interface IPlatformService
{
    public Task<UserInfo> GetUserInfo(string tokenString);
}