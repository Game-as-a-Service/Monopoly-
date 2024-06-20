using Monopoly.DomainLayer.Domain;

namespace Monopoly.ApplicationLayer.Application.DataModels;

public record MonopolyDataModel(string Id, Player[] Players, Map Map, string HostId, CurrentPlayerState CurrentPlayerState, LandHouse[] LandHouses);

public record Player(string Id, decimal Money, Chess Chess, LandContract[] LandContracts, PlayerState PlayerState, int BankruptRounds, int LocationId, string? RoleId);
public record CurrentPlayerState(string PlayerId, bool IsPayToll, bool IsBoughtLand, bool IsUpgradeLand, Auction? Auction, int RemainingSteps, bool HadSelectedDirection);
public record Chess(string CurrentPosition, Direction Direction);
public record LandContract(string LandId, bool InMortgage, int Deadline);
public record Auction(string LandId, string? HighestBidderId, decimal HighestPrice);
public record LandHouse(string LandId, int House);
public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

public record Map(string Id, Block?[][] Blocks);

public abstract record Block(string Id);

public record EmptyBlock() : Block(string.Empty);

public record Land(string Id) : Block(Id);

public record ParkingLot(string Id) : Block(Id);

public record Jail(string Id) : Block(Id);

public record StartPoint(string Id) : Block(Id);

public record Station(string Id) : Land(Id);

public record Road(string Id) : Block(Id);