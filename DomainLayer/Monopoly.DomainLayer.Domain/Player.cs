using Monopoly.DomainLayer.Common;
using Monopoly.DomainLayer.Domain.Events;
using Monopoly.DomainLayer.Domain.Interfaces;

namespace Monopoly.DomainLayer.Domain;

public class Player
{
    private decimal money;
    private Chess chess;
    private readonly List<LandContract> _landContractList = new();

    public Player(string id, decimal money = 15000, PlayerState playerState = PlayerState.Normal, int bankruptRounds = 0, int locationId = 0, string? roleId = null)
    {
        Id = id;
        Money = money;
        State = playerState;
        BankruptRounds = bankruptRounds;
        LocationId = locationId;
        RoleId = roleId;
    }

    public PlayerState State { get; internal set; }
    public MonopolyAggregate MonopolyAggregate { get; internal set; }
    public string Id { get; }
    public decimal Money
    {
        get { return money; }
        set
        {
            money = (int)value;
            if (money < 0)
            {
                money = 0;
            }
        }
    }

    public IList<LandContract> LandContractList => _landContractList.AsReadOnly();

    public Chess Chess { get => chess; set => chess = value; }
    public bool EndRoundFlag { get; set; }
    // false: 回合尚不能結束，true: 玩家可結束回合
    public int SuspendRounds { get; private set; } = 0;
    public int BankruptRounds { get; set; }

    internal DomainEvent? UpdateState()
    {
        if (Money <= 0 && !LandContractList.Any(l => !l.InMortgage))
        {
            State = PlayerState.Bankrupt;
            foreach (var landContract in LandContractList)
            {
                RemoveLandContract(landContract);
            }
            EndRoundFlag = true;
            return new PlayerBankruptEvent(Id);
        }
        return null;
    }

    public string? RoleId { get; set; }

    public int LocationId { get; set; }

    public void AddLandContract(LandContract landContract)
    {
        _landContractList.Add(landContract);
        landContract.Land.UpdateOwner(this);
    }

    internal void RemoveLandContract(LandContract landContract)
    {
        landContract.Land.UpdateOwner(null);
        _landContractList.Remove(landContract);
    }

    public LandContract? FindLandContract(string id)
    {
        return LandContractList.Where(landContract => landContract.Land.Id == id).FirstOrDefault();
    }

    public void SuspendRound(string reason)
    {
        SuspendRounds = reason switch
        {
            "Jail" => 2,
            "ParkingLot" => 1,
            _ => 0,
        };
    }

    public List<DomainEvent> EndRound()
    {
        var events = _landContractList
            .Select(l => l.EndRound())
            .Where(x => x is not null)
            .OfType<DomainEvent>()
            .ToList();

        _landContractList.RemoveAll(l => l.Deadline == 0);

        return events;
    }

    internal void StartRound()
    {
        EndRoundFlag = true;

        if (SuspendRounds > 0)
        {
            SuspendRounds--;
        }
    }

    internal IEnumerable<DomainEvent> RollDice(Map map, IDice[] dices)
    {
        foreach (var dice in dices)
        {
            dice.Roll();
        }
        yield return new PlayerRolledDiceEvent(Id, [.. dices.Select(d => d.Value)]);
        var events = chess.Move(map, dices.Sum(dice => dice.Value));

        foreach (var e in events)
        {
            yield return e;
        }
    }

    internal IEnumerable<DomainEvent> SelectDirection(Map map, Map.Direction direction)
    {
        var events = chess.ChangeDirection(map, direction);
        return events;
    }

    internal DomainEvent MortgageLandContract(string landId)
    {
        // 玩家擁有地契並尚未抵押
        if (_landContractList.Exists(l => l.Land.Id == landId && !l.InMortgage))
        {
            var landContract = _landContractList.First(l => l.Land.Id == landId);
            landContract.GetMortgage();
            Money += landContract.Land.GetPrice("Mortgage");
            return new PlayerMortgageEvent(Id, Money,
                                            landId,
                                            landContract.Deadline);
        }
        else
        {
            return new PlayerCannotMortgageEvent(Id, Money, landId);
        }
    }

    internal DomainEvent RedeemLandContract(string landId)
    {
        // 玩家擁有地契並正在抵押
        if (_landContractList.Exists(l => l.Land.Id == landId && l.InMortgage))
        {
            var landContract = _landContractList.First(l => l.Land.Id == landId);
            var RedeemMoney = landContract.Land.GetPrice("Redeem");
            if (Money >= RedeemMoney)
            {
                landContract.GetRedeem();
                Money -= RedeemMoney;
                return new PlayerRedeemEvent(Id, Money, landId);
            }
            else
            {
                return new PlayerTooPoorToRedeemEvent(Id, Money, landId, RedeemMoney);
            }
        }
        else
        {
            return new LandNotInMortgageEvent(Id, landId);
        }
    }

    internal void PayToll(Player owner, decimal amount)
    {
        Money -= amount;
        owner.Money += amount;
    }

    internal DomainEvent BuildHouse(Map map)
    {
        Block block = map.FindBlockById(chess.CurrentBlockId);
        if (block is not Land land)
        {
            return new PlayerCannotBuildHouseEvent(Id, block.Id);
        }
        // 玩家站在該土地上
        else if (Chess.CurrentBlockId != land.Id)
        {
            return new PlayerBuyBlockMissedLandEvent(Id, land.Id);
        }
        // 玩家擁有該土地
        else if (land.GetOwner() != this)
        {
            return new PlayerCannotBuildHouseEvent(Id, land.Id);
        }
        // 此土地沒有被抵押
        else if (LandContractList.Any(l => l.Land == land && l.InMortgage))
        {
            return new PlayerCannotBuildHouseEvent(Id, land.Id);
        }
        // 玩家有足夠的錢
        else if (land.UpgradePrice > Money)
        {
            return new PlayerTooPoorToBuildHouseEvent(Id, land.Id, Money, land.UpgradePrice);
        }
        // 玩家這回合沒有買地
        else if (MonopolyAggregate.CurrentPlayerState.IsBoughtLand)
        {
            return new PlayerCannotBuildHouseEvent(Id, land.Id);
        }
        // 玩家這回合沒有蓋房子
        else if (MonopolyAggregate.CurrentPlayerState.IsUpgradeLand)
        {
            return new PlayerCannotBuildHouseEvent(Id, land.Id);
        }
        else
        {
            return land.BuildHouse(this);
        }
    }

    internal List<DomainEvent> BuyLand(Map map, string BlockId)
    {
        List<DomainEvent> events = new();

        //判斷是否為可購買土地
        if (map.FindBlockById(chess.CurrentBlockId) is not Land land)
        {
            events.Add(new PlayerBuyBlockOccupiedByOtherPlayerEvent(Id, BlockId));
        }
        //判斷是否非空地
        else if (land.GetOwner() is not null)
        {
            events.Add(new PlayerBuyBlockOccupiedByOtherPlayerEvent(Id, BlockId));
        }
        //判斷是否踩在該土地
        else if (Chess.CurrentBlockId != BlockId)
        {
            events.Add(new PlayerBuyBlockMissedLandEvent(Id, BlockId));
        }
        //判斷格子購買金額足夠
        else if (land.Price > Money)
        {
            events.Add(new PlayerBuyBlockInsufficientFundsEvent(Id, BlockId, land.Price));
        }
        else
        {
            //玩家扣款
            Money -= land.Price;

            //過戶(?
            var landContract = new LandContract(this, land);
            AddLandContract(landContract);

            events.Add(new PlayerBuyBlockEvent(Id, BlockId));

        }

        return events;
    }

    internal bool CanNotSelectDirection(Map.Direction d)
    {
        return d == chess.CurrentDirection.Opposite();
    }
}