using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.ApplicationLayer.Application.DataModels;
using Monopoly.DomainLayer.Domain;
using Monopoly.DomainLayer.Domain.Interfaces;
using Monopoly.InterfaceAdapterLayer.Server.Tests.Usecases;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;
using Auction = Monopoly.ApplicationLayer.Application.DataModels.Auction;
using Chess = Monopoly.ApplicationLayer.Application.DataModels.Chess;
using CurrentPlayerState = Monopoly.ApplicationLayer.Application.DataModels.CurrentPlayerState;
using GameStage = Monopoly.ApplicationLayer.Application.DataModels.GameStage;
using LandContract = Monopoly.ApplicationLayer.Application.DataModels.LandContract;
using Map = Monopoly.ApplicationLayer.Application.DataModels.Map;
using Player = Monopoly.ApplicationLayer.Application.DataModels.Player;

namespace Monopoly.InterfaceAdapterLayer.Server.Tests;

public class Utils
{
    //public static IDice[]? MockDice(params int[] diceValues)
    //{
    //    var dice = new IDice[diceValues.Length];
    //    for (int i = 0; i < diceValues.Length; i++)
    //    {
    //        var mockDice = new Mock<IDice>();
    //        mockDice.Setup(x => x.Roll());
    //        mockDice.Setup(x => x.Value).Returns(diceValues[i]);
    //        dice[i] = mockDice.Object;
    //    }

    //    return dice;
    //}

    internal static void VerifyChessMovedEvent(VerificationHub hub, string playerId, string blockId, string direction,
        int remainingSteps)
    {
        hub.Verify(nameof(IMonopolyResponses.ChessMovedEvent), (ChessMovedEventArgs e) =>
            e.PlayerId == playerId && e.BlockId == blockId && e.Direction == direction &&
            e.RemainingSteps == remainingSteps);
    }

    public class MonopolyBuilder
    {
        private string GameId { get; set; }

        private List<Player> Players { get; set; } = new();

        private string HostId { get; set; }

        private int[] Dices { get; set; } = [0];

        private CurrentPlayerState CurrentPlayerState { get; set; }
        private List<LandHouse> LandHouses { get; set; } = [];
        public Map Map { get; private set; }
        private GameStage GameStage { get; set; }

        public MonopolyBuilder(string id)
        {
            GameId = id;
            GameStage = GameStage.Gaming;
        }

        public MonopolyBuilder WithPlayer(Player player)
        {
            Players.Add(player);
            return this;
        }

        public MonopolyBuilder WithMockDice(int[] dices)
        {
            Dices = dices;
            return this;
        }

        public MonopolyBuilder WithCurrentPlayer(CurrentPlayerState currentPlayerState)
        {
            CurrentPlayerState = currentPlayerState;
            return this;
        }

        public MonopolyBuilder WithHost(string id)
        {
            HostId = id;
            return this;
        }

        public MonopolyBuilder WithLandHouse(string id, int house)
        {
            LandHouses.Add(new LandHouse(id, house));
            return this;
        }

        private MonopolyDataModel Build()
        {
            return new MonopolyDataModel(Id: GameId,
                Players: [..Players],
                Map: Map,
                HostId: HostId,
                GameStage: GameStage,
                CurrentPlayerState: CurrentPlayerState,
                LandHouses: LandHouses.ToArray());
        }

        internal void Save(MonopolyTestServer server)
        {
            var monopoly = Build();
            server.GetRequiredService<ICommandRepository>().Save(monopoly);
            server.GetRequiredService<MockDiceService>().Dices =
                Dices.Select(value => new MockDice(value)).ToArray<IDice>();
        }

        internal MonopolyBuilder WithGameStage(GameStage gameStage)
        {
            GameStage = gameStage;
            return this;
        }
    }

    public class PlayerBuilder(string id)
    {
        private string Id { get; set; } = id;
        private decimal Money { get; set; } = 15000;
        private string BlockId { get; set; } = "StartPoint";
        private Direction Direction { get; set; } = Direction.Right;
        private List<LandContract> LandContracts { get; set; } = new();
        private PlayerState PlayerState { get; set; } = PlayerState.Normal;
        private int BankruptRounds { get; set; }
        private string? RoleId { get; set; }
        private int LocationId { get; set; }

        public PlayerBuilder WithMoney(decimal money)
        {
            Money = money;
            return this;
        }

        public PlayerBuilder WithPosition(string blockId, Direction direction)
        {
            BlockId = blockId;
            Direction = direction;
            return this;
        }

        public PlayerBuilder WithLandContract(string landId, bool inMortgage = false, int deadline = 10)
        {
            LandContracts.Add(new LandContract(
                LandId: landId,
                InMortgage: inMortgage,
                Deadline: deadline));
            return this;
        }

        public PlayerBuilder WithBankrupt(int rounds)
        {
            PlayerState = PlayerState.Bankrupt;
            BankruptRounds = rounds;
            return this;
        }

        public Player Build()
        {
            Chess chess = new(CurrentPosition: BlockId,
                Direction: Enum.Parse<ApplicationLayer.Application.DataModels.Direction>(Direction.ToString()));
            Player player = new(Id: Id,
                Money: Money,
                Chess: chess,
                LandContracts: LandContracts.ToArray(),
                PlayerState: PlayerState,
                BankruptRounds,
                LocationId: LocationId,
                RoleId: RoleId
            );
            return player;
        }

        internal PlayerBuilder WithRole(string roleId)
        {
            RoleId = roleId;
            return this;
        }

        internal PlayerBuilder WithLocation(int locationId)
        {
            LocationId = locationId;
            return this;
        }

        internal PlayerBuilder WithState(PlayerState playerState)
        {
            PlayerState = playerState;
            return this;
        }
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public class CurrentPlayerStateBuilder(string id)
    {
        private string Id { get; set; } = id;
        private bool IsPayToll { get; set; }
        private bool IsBoughtLand { get; set; }
        private bool IsUpgradeLand { get; set; }
        private Auction? Auction { get; set; }
        private int RemainingSteps { get; set; }
        private bool HadSelectedDirection { get; set; }

        public CurrentPlayerStateBuilder WithPayToll()
        {
            IsPayToll = true;
            return this;
        }

        public CurrentPlayerStateBuilder WithBoughtLand()
        {
            IsBoughtLand = true;
            return this;
        }

        public CurrentPlayerStateBuilder WithUpgradeLand()
        {
            IsUpgradeLand = true;
            return this;
        }

        internal CurrentPlayerStateBuilder WithAuction(string landId, string? highestBidderId, decimal highestPrice)
        {
            Auction = new Auction(landId, highestBidderId, highestPrice);
            return this;
        }

        internal CurrentPlayerStateBuilder WithRemainingSteps(int remainingSteps)
        {
            RemainingSteps = remainingSteps;
            return this;
        }

        internal CurrentPlayerStateBuilder HadNotSelectedDirection()
        {
            HadSelectedDirection = false;
            return this;
        }

        public CurrentPlayerState Build()
        {
            return new CurrentPlayerState(PlayerId: Id,
                IsPayToll: IsPayToll,
                IsBoughtLand: IsBoughtLand,
                IsUpgradeLand: IsUpgradeLand,
                Auction: Auction,
                RemainingSteps: RemainingSteps,
                HadSelectedDirection: HadSelectedDirection);
        }
    }
}