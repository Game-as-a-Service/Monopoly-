namespace SharedLibrary.ResponseArgs.ReadyRoom.Models;

public record Player(string Id, string Name, bool IsReady, LocationEnum Location, RoleEnum Role);