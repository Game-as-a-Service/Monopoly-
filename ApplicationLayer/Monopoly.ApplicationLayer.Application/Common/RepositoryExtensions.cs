using Monopoly.ApplicationLayer.Application.DataModels;
using Monopoly.DomainLayer.Domain;
using Monopoly.DomainLayer.Domain.Builders;
using Monopoly.DomainLayer.Domain.Maps;
using Auction = Monopoly.ApplicationLayer.Application.DataModels.Auction;
using Block = Monopoly.ApplicationLayer.Application.DataModels.Block;
using Chess = Monopoly.ApplicationLayer.Application.DataModels.Chess;
using CurrentPlayerState = Monopoly.ApplicationLayer.Application.DataModels.CurrentPlayerState;
using Jail = Monopoly.DomainLayer.Domain.Jail;
using Land = Monopoly.DomainLayer.Domain.Land;
using LandContract = Monopoly.ApplicationLayer.Application.DataModels.LandContract;
using Map = Monopoly.ApplicationLayer.Application.DataModels.Map;
using ParkingLot = Monopoly.DomainLayer.Domain.ParkingLot;
using Player = Monopoly.ApplicationLayer.Application.DataModels.Player;
using Road = Monopoly.DomainLayer.Domain.Road;
using StartPoint = Monopoly.DomainLayer.Domain.StartPoint;
using Station = Monopoly.DomainLayer.Domain.Station;

namespace Monopoly.ApplicationLayer.Application.Common;

public static class RepositoryExtensions
{
    /// <summary>
    /// (Monopoly) Domain to Application
    /// </summary>
    /// <param name="domainMonopolyAggregate"></param>
    /// <returns></returns>
    public static MonopolyDataModel ToApplication(this MonopolyAggregate domainMonopolyAggregate)
    {
        var players = domainMonopolyAggregate.Players.Select(player =>
        {
            var playerChess = player.Chess;

            Chess chess = new(playerChess.CurrentBlockId, playerChess.CurrentDirection.ToApplicationDirection());

            var landContracts = player.LandContractList.Select(contract =>
                new LandContract(contract.Land.Id, contract.InMortgage, contract.Deadline)).ToArray();

            return new Player(
                player.Id,
                player.Money,
                chess,
                landContracts,
                player.State,
                player.BankruptRounds,
                player.LocationId,
                player.RoleId
            );
        }).ToArray();

        Map map = new(domainMonopolyAggregate.Map.Id, domainMonopolyAggregate.Map.Blocks
            .Select(row => { return row.Select(block => block?.ToApplicationBlock()).ToArray(); }).ToArray()
        );
        var currentPlayer =
            domainMonopolyAggregate.Players.First(player =>
                player.Id == domainMonopolyAggregate.CurrentPlayerState.PlayerId);
        var auction = domainMonopolyAggregate.CurrentPlayerState.Auction;
        var currentPlayerState = new CurrentPlayerState(
            domainMonopolyAggregate.CurrentPlayerState.PlayerId,
            domainMonopolyAggregate.CurrentPlayerState.IsPayToll,
            domainMonopolyAggregate.CurrentPlayerState.IsBoughtLand,
            domainMonopolyAggregate.CurrentPlayerState.IsUpgradeLand,
            domainMonopolyAggregate.CurrentPlayerState.Auction is null
                ? null
                : new Auction(auction!.LandContract.Land.Id, auction.HighestBidder?.Id, auction.HighestPrice),
            domainMonopolyAggregate.CurrentPlayerState.RemainingSteps,
            domainMonopolyAggregate.CurrentPlayerState.HadSelectedDirection
        );
        var LandHouses = domainMonopolyAggregate.Map.Blocks.SelectMany(block => block).OfType<Land>()
            .Where(land => land.House > 0)
            .Select(land => new LandHouse(land.Id, land.House)).ToArray();


        return new MonopolyDataModel(domainMonopolyAggregate.Id, players, map, domainMonopolyAggregate.HostId,
            currentPlayerState, LandHouses);
    }

    private static Block ToApplicationBlock(this DomainLayer.Domain.Block domainBlock)
    {
        return domainBlock switch
        {
            StartPoint startBlock => new DataModels.StartPoint(startBlock.Id),
            Station stationBlock => new DataModels.Station(stationBlock.Id),
            Land landBlock => new DataModels.Land(landBlock.Id),
            ParkingLot parkingLotBlock => new DataModels.ParkingLot(parkingLotBlock.Id),
            Jail prisonBlock => new DataModels.Jail(prisonBlock.Id),
            Road roadBlock => new DataModels.Road(roadBlock.Id),
            null => new EmptyBlock(),
            _ => throw new NotImplementedException(),
        };
    }

    /// <summary>
    /// (Monopoly) Application to Domain
    /// </summary>
    /// <param name="monopolyDataModel"></param>
    /// <returns></returns>
    public static MonopolyAggregate ToDomain(this MonopolyDataModel monopolyDataModel)
    {
        //Domain.Map map = new(monopoly.Map.Id, monopoly.Map.Blocks
        //               .Select(row =>
        //               {
        //                   return row.Select(block => block?.ToDomainBlock()).ToArray();
        //               }).ToArray()
        //           );
        DomainLayer.Domain.Map map = new SevenXSevenMap();
        var builder = new MonopolyBuilder()
            .WithId(monopolyDataModel.Id)
            .WithHost(monopolyDataModel.HostId)
            .WithMap(map);
        monopolyDataModel.Players.ToList().ForEach(
            p => builder.WithPlayer(p.Id, playerBuilder =>
                playerBuilder.WithMoney(p.Money)
                    .WithPosition(p.Chess.CurrentPosition, p.Chess.Direction.ToString())
                    .WithLandContracts(p.LandContracts)
                    .WithBankrupt(p.BankruptRounds)
                    .WithLocation(p.LocationId)
                    .WithRole(p.RoleId)
                    .WithState(p.PlayerState)
            ));

        var cps = monopolyDataModel.CurrentPlayerState;
        if (cps.Auction is null)
        {
            builder.WithCurrentPlayer(cps.PlayerId, x => x.WithBoughtLand(cps.IsBoughtLand)
                .WithUpgradeLand(cps.IsUpgradeLand)
                .WithPayToll(cps.IsPayToll)
                .WithSelectedDirection(cps.HadSelectedDirection));
        }
        else
        {
            builder.WithCurrentPlayer(cps.PlayerId, x => x.WithAuction(
                cps.Auction.LandId, cps.Auction.HighestBidderId, cps.Auction.HighestPrice));
        }

        monopolyDataModel.LandHouses.ToList().ForEach(LandHouse => builder.WithLandHouse(LandHouse.LandId, LandHouse.House));

        return builder.Build();
    }

    private static PlayerBuilder WithLandContracts(this PlayerBuilder builder,
        LandContract[] landContracts)
    {
        landContracts.ToList().ForEach(landContract =>
        {
            builder.WithLandContract(landContract.LandId, landContract.InMortgage, landContract.Deadline);
        });
        return builder;
    }

    private static Direction ToApplicationDirection(this DomainLayer.Domain.Map.Direction direction)
    {
        return direction switch
        {
            DomainLayer.Domain.Map.Direction.Up => Direction.Up,
            DomainLayer.Domain.Map.Direction.Down => Direction.Down,
            DomainLayer.Domain.Map.Direction.Left => Direction.Left,
            DomainLayer.Domain.Map.Direction.Right => Direction.Right,
            _ => throw new NotImplementedException(),
        };
    }
}