using System.Collections.Immutable;

namespace SharedLibrary.ResponseArgs.Monopoly.Models;

public record MonopolyInfos(string WhoAmI, string HostId, string CurrentPlayerId, ImmutableArray<MonopolyInfos.PlayerInfos> Players)
{
    public record PlayerInfos(string Id, string Name, string Color, string Role);
}