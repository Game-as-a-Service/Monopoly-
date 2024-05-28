namespace Application.Common;

public abstract record BaseRequest;
public abstract record GameRequest(string GameId, string PlayerId): BaseRequest;