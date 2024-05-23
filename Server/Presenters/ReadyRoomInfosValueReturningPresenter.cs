using System.Collections.Immutable;
using Application.Queries;
using Monopoly.DomainLayer.ReadyRoom;
using SharedLibrary.ResponseArgs.ReadyRoom.Models;
using Player = SharedLibrary.ResponseArgs.ReadyRoom.Models.Player;

using DomainLocationEnum = Monopoly.DomainLayer.ReadyRoom.Enums.LocationEnum;
using ServerLocationEnum = SharedLibrary.ResponseArgs.ReadyRoom.Models.LocationEnum;

using ServerRoleEnum = SharedLibrary.ResponseArgs.ReadyRoom.Models.RoleEnum;

namespace Server.Presenters;

public class ReadyRoomInfosValueReturningPresenter : ValueReturningPresenter<ReadyRoomInfos, GetReadyRoomInfosResponse>
{
    protected override Task<ReadyRoomInfos> TransformAsync(GetReadyRoomInfosResponse response, CancellationToken cancellationToken)
    {
        var readyRoom = response.ReadyRoom;
        var players = readyRoom.Players.Select(p => new Player(p.Id, p.Name, p.IsReady, p.Location.AsServerLocationEnum(),
            ServerEnumExtensions.AsServerRoleEnum(p.RoleId)
        ));
        var readyRoomInfos = new ReadyRoomInfos(
            [
                ..players
            ],
            readyRoom.HostId
        );
        return Task.FromResult(readyRoomInfos);
    }
}

public static class ServerEnumExtensions
{
    public static ServerLocationEnum AsServerLocationEnum(this DomainLocationEnum location)
    {
        return location switch
        {
            DomainLocationEnum.First => ServerLocationEnum.First,
            DomainLocationEnum.Second => ServerLocationEnum.Second,
            DomainLocationEnum.Third => ServerLocationEnum.Third,
            DomainLocationEnum.Fourth => ServerLocationEnum.Fourth,
            DomainLocationEnum.None => ServerLocationEnum.None,
            _ => throw new ArgumentOutOfRangeException(nameof(location), location, null)
        };
    }
    
    public static ServerRoleEnum AsServerRoleEnum(string roleId)
    {
        return roleId switch
        {
            "Mei" => ServerRoleEnum.Mei,
            "OldMan" => ServerRoleEnum.OldMan,
            "Baby" => ServerRoleEnum.Baby,
            "Dai" => ServerRoleEnum.Dai,
            "" => ServerRoleEnum.None,
            _ => throw new ArgumentOutOfRangeException(nameof(roleId), roleId, null)
        };
    }
    
    public static DomainLocationEnum AsDomainLocationEnum(this ServerLocationEnum location)
    {
        return location switch
        {
            ServerLocationEnum.First => DomainLocationEnum.First,
            ServerLocationEnum.Second => DomainLocationEnum.Second,
            ServerLocationEnum.Third => DomainLocationEnum.Third,
            ServerLocationEnum.Fourth => DomainLocationEnum.Fourth,
            ServerLocationEnum.None => DomainLocationEnum.None,
            _ => throw new ArgumentOutOfRangeException(nameof(location), location, null)
        };
    }
    
    public static string AsDomainRoleId(this ServerRoleEnum role)
    {
        return role switch
        {
            ServerRoleEnum.Mei => "Mei",
            ServerRoleEnum.OldMan => "OldMan",
            ServerRoleEnum.Baby => "Baby",
            ServerRoleEnum.Dai => "Dai",
            ServerRoleEnum.None => "",
            _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
        };
    }
}