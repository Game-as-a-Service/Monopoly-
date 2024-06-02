using System.Collections.Immutable;

namespace SharedLibrary.ResponseArgs.ReadyRoom.Models;

public record ReadyRoomInfos(string RequestPlayerId, ImmutableArray<Player> Players, string HostId);