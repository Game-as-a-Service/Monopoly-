using System.Collections.Immutable;

namespace SharedLibrary.ResponseArgs.ReadyRoom.Models;

public record ReadyRoomInfos(ImmutableArray<Player> Players, string HostId);