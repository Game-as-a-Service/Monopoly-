using Application.DataModels;
using Domain.Maps;

namespace Application.Common;

internal static class RepositoryExtensions
{
    internal static string Save(this IRepository repository, Domain.MonopolyAggregate domainMonopolyAggregate)
    {
        var monopoly = domainMonopolyAggregate.ToApplication();
        return repository.Save(monopoly);
    }

    /// <summary>
    /// (Monopoly) Domain to Application
    /// </summary>
    /// <param name="domainMonopolyAggregate"></param>
    /// <returns></returns>
    private static MonopolyDataModel ToApplication(this Domain.MonopolyAggregate domainMonopolyAggregate)
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
        var gamestage = domainMonopolyAggregate.GameStage switch
        {
            Domain.GameStage.Ready => GameStage.Preparing,
            Domain.GameStage.Gaming => GameStage.Gaming,
            _ => throw new NotImplementedException(),
        };
        if (gamestage == GameStage.Preparing)
        {
            return new MonopolyDataModel(domainMonopolyAggregate.Id, [..players], map, domainMonopolyAggregate.HostId, null!,
                null!, gamestage);
        }

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
        var LandHouses = domainMonopolyAggregate.Map.Blocks.SelectMany(block => block).OfType<Domain.Land>()
            .Where(land => land.House > 0)
            .Select(land => new LandHouse(land.Id, land.House)).ToArray();


        return new DataModels.MonopolyDataModel(domainMonopolyAggregate.Id, players, map, domainMonopolyAggregate.HostId,
            currentPlayerState, LandHouses, gamestage);
    }

    private static Block ToApplicationBlock(this Domain.Block domainBlock)
    {
        return domainBlock switch
        {
            Domain.StartPoint startBlock => new StartPoint(startBlock.Id),
            Domain.Station stationBlock => new Station(stationBlock.Id),
            Domain.Land landBlock => new Land(landBlock.Id),
            Domain.ParkingLot parkingLotBlock => new ParkingLot(parkingLotBlock.Id),
            Domain.Jail prisonBlock => new Jail(prisonBlock.Id),
            Domain.Road roadBlock => new Road(roadBlock.Id),
            null => new EmptyBlock(),
            _ => throw new NotImplementedException(),
        };
    }

    /// <summary>
    /// (Monopoly) Application to Domain
    /// </summary>
    /// <param name="monopolyDataModel"></param>
    /// <returns></returns>
    internal static Domain.MonopolyAggregate ToDomain(this MonopolyDataModel monopolyDataModel)
    {
        //Domain.Map map = new(monopoly.Map.Id, monopoly.Map.Blocks
        //               .Select(row =>
        //               {
        //                   return row.Select(block => block?.ToDomainBlock()).ToArray();
        //               }).ToArray()
        //           );
        Domain.Map map = new SevenXSevenMap();
        var builder = new Domain.Builders.MonopolyBuilder()
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
        builder.WithGameStage(monopolyDataModel.GameStage switch
        {
            GameStage.Preparing => Domain.GameStage.Ready,
            GameStage.Gaming => Domain.GameStage.Gaming,
            _ => throw new NotImplementedException(),
        });
        if (monopolyDataModel.GameStage == GameStage.Preparing)
        {
            return builder.Build();
        }

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

    private static Domain.Builders.PlayerBuilder WithLandContracts(this Domain.Builders.PlayerBuilder builder,
        LandContract[] landContracts)
    {
        landContracts.ToList().ForEach(landContract =>
        {
            builder.WithLandContract(landContract.LandId, landContract.InMortgage, landContract.Deadline);
        });
        return builder;
    }

    private static Domain.Block? ToDomainBlock(this Block? block)
    {
        return block switch
        {
            StartPoint startBlock => new Domain.StartPoint(startBlock.Id),
            Station stationBlock => new Domain.Station(stationBlock.Id),
            Land landBlock => new Domain.Land(landBlock.Id),
            ParkingLot parkingLotBlock => new Domain.ParkingLot(parkingLotBlock.Id),
            Jail prisonBlock => new Domain.Jail(prisonBlock.Id),
            Road roadBlock => new Domain.Road(roadBlock.Id),
            EmptyBlock => null,
            _ => throw new NotImplementedException(),
        };
    }

    private static Direction ToApplicationDirection(this Domain.Map.Direction direction)
    {
        return direction switch
        {
            Domain.Map.Direction.Up => Direction.Up,
            Domain.Map.Direction.Down => Direction.Down,
            Domain.Map.Direction.Left => Direction.Left,
            Domain.Map.Direction.Right => Direction.Right,
            _ => throw new NotImplementedException(),
        };
    }
}