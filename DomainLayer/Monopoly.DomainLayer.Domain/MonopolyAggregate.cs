using Monopoly.DomainLayer.Common;
using Monopoly.DomainLayer.Domain.Builders;
using Monopoly.DomainLayer.Domain.Events;
using Monopoly.DomainLayer.Domain.Interfaces;
using static Monopoly.DomainLayer.Domain.Map;

namespace Monopoly.DomainLayer.Domain;

public class MonopolyAggregate : AggregateRoot
{
    public string Id { get; set; }
    public int[]? CurrentDice { get; set; } = null;
    public CurrentPlayerState CurrentPlayerState => _currentPlayerState;
    public IDice[] Dices { set; get; }
    private Player CurrentPlayer => _players.First(p => p.Id == _currentPlayerState.PlayerId);

    private readonly Map _map;
    private readonly List<Player> _players = new();
    private CurrentPlayerState _currentPlayerState;

    public ICollection<Player> Players => _players.AsReadOnly();

    public string HostId { get; init; }

    public Map Map => _map;

    public int Rounds { get; private set; }


    internal MonopolyAggregate(string gameId, Player[] players, Map map, string hostId, CurrentPlayerState currentPlayerState, IDice[]? dices = null, int rounds = 0) 
        : base(gameId)
    {
        Id = gameId;
        _players = players.ToList();
        _map = map;
        HostId = hostId;
        _currentPlayerState = currentPlayerState;
        Dices = dices ?? new IDice[2] { new Dice(), new Dice() };

        foreach (var player in _players)
        {
            player.MonopolyAggregate = this;
        }

        Rounds = rounds;
    }

    public void Settlement()
    {
        // 玩家資產計算方式: 土地價格+升級價格+剩餘金額 
        // 抵押的房地產不列入計算
        var PropertyCalculate = (Player player) =>
            player.Money + player.LandContractList.Where(l => !l.InMortgage).Sum(l => (l.Land.House + 1) * l.Land.Price);
        // 根據玩家資產進行排序，多的在前，若都已經破產了，則以破產時間晚的在前
        var players = _players.OrderByDescending(PropertyCalculate).ThenByDescending(p => p.BankruptRounds).ToArray();
        AddDomainEvent(new GameSettlementEvent(Rounds, players));
    }

    public Block GetPlayerPosition(string playerId)
    {
        Player player = GetPlayer(playerId);
        return _map.FindBlockById(player.Chess.CurrentBlockId);
    }

    // 玩家選擇方向
    // 1.不能選擇回頭的方向
    // 2.不能選擇沒有的方向
    public void PlayerSelectDirection(string playerId, string direction)
    {
        Player player = GetPlayer(playerId);
        VerifyCurrentPlayer(player);
        if (CurrentPlayerState.HadSelectedDirection)
        {
            AddDomainEvent(new PlayerHadSelectedDirectionEvent(playerId));
            return;
        }
        var d = GetDirection(direction);
        if (CurrentPlayer.CanNotSelectDirection(d))
        {
            AddDomainEvent(new PlayerChooseInvalidDirectionEvent(playerId, direction));
            return;
        }
        var events = player.SelectDirection(_map, d);
        AddDomainEvent(events);
    }

    private static Direction GetDirection(string direction)
    {
        return direction switch
        {
            "Up" => Direction.Up,
            "Down" => Direction.Down,
            "Left" => Direction.Left,
            "Right" => Direction.Right,
            _ => throw new Exception("方向錯誤")
        };
    }

    public void Initial()
    {
        _players.ForEach(x => x.State = PlayerState.Normal);
        // 初始化目前玩家
        _currentPlayerState = new CurrentPlayerStateBuilder(_players[0].Id).Build(null);
        CurrentPlayer.StartRound();
    }

    /// <summary>
    /// 擲骰子
    /// 並且移動棋子直到遇到需要選擇方向的地方
    /// </summary>
    /// <param name="playerId"></param>
    /// <exception cref="Exception"></exception>
    public void PlayerRollDice(string playerId)
    {
        Player player = GetPlayer(playerId);
        VerifyCurrentPlayer(player);
        var events = player.RollDice(_map, Dices);
        AddDomainEvent(events);
    }

    public void EndAuction()
    {
        if (CurrentPlayerState.Auction is null)
        {
            throw new Exception("沒有拍賣");
        }
        var domainEvent = _currentPlayerState.Auction!.End();
        _currentPlayerState = _currentPlayerState with { Auction = null };
        
        AddDomainEvent(domainEvent);
    }

    public void PlayerSellLandContract(string playerId, string landId)
    {
        Player player = GetPlayer(playerId);
        VerifyCurrentPlayer(player);
        var landContract = player.FindLandContract(landId) ?? throw new Exception("找不到地契");
        _currentPlayerState = CurrentPlayerState with { Auction = new Auction(landContract) };
    }

    public void PlayerBid(string playerId, decimal price)
    {
        Player player = GetPlayer(playerId);
        if (playerId == CurrentPlayer.Id)
        {
            AddDomainEvent(new CurrentPlayerCannotBidEvent(playerId));
        }
        else
        {
            AddDomainEvent(_currentPlayerState?.Auction.Bid(player, price));
        }
    }

    public void MortgageLandContract(string playerId, string landId)
    {
        Player player = GetPlayer(playerId);
        VerifyCurrentPlayer(player);
        AddDomainEvent(player.MortgageLandContract(landId));
    }

    public void RedeemLandContract(string playerId, string landId)
    {
        Player player = GetPlayer(playerId);
        VerifyCurrentPlayer(player);
        AddDomainEvent(player.RedeemLandContract(landId));
    }

    public void PayToll(string playerId)
    {
        Player player = GetPlayer(playerId);
        VerifyCurrentPlayer(player);
        Land location = (Land)GetPlayerPosition(player.Id);
        if (CurrentPlayerState.IsPayToll)
        {
            AddDomainEvent(new PlayerDoesntNeedToPayTollEvent(player.Id, player.Money));
            return;
        }

        var domainEvent = location.PayToll(player);

        AddDomainEvent(domainEvent);
    }

    public void BuildHouse(string playerId)
    {
        Player player = GetPlayer(playerId);
        VerifyCurrentPlayer(player);
        AddDomainEvent(player.BuildHouse(_map));
    }

    public void EndRound()
    {
        if (CurrentPlayerState.CanEndRound)
        {
            // 結束回合，輪到下一個玩家
            AddDomainEvent(CurrentPlayer.EndRound());
            string lastPlayerId = CurrentPlayer.Id;
            do
            {
                _currentPlayerState = new CurrentPlayerStateBuilder(_players[(_players.IndexOf(CurrentPlayer) + 1) % _players.Count].Id).Build(null);
                CurrentPlayer.StartRound();
            } while (CurrentPlayer.State == PlayerState.Bankrupt);
            AddDomainEvent(new EndRoundEvent(lastPlayerId, CurrentPlayer.Id));
            if (CurrentPlayer.SuspendRounds > 0)
            {
                _currentPlayerState = _currentPlayerState with { IsPayToll = true }; // 當玩家暫停回合時，不需要繳過路費。 FIX: 這很奇怪
                AddDomainEvent(new SuspendRoundEvent(CurrentPlayer.Id, CurrentPlayer.SuspendRounds));
                EndRound();
            }
        }
        else
        {
            AddDomainEvent(new EndRoundFailEvent(CurrentPlayer.Id));
        }
    }

    #region Private Functions

    private Player GetPlayer(string id)
    {
        var player = _players.Find(p => p.Id == id) ?? throw new Exception("找不到玩家");
        return player;
    }

    private void VerifyCurrentPlayer(Player? player)
    {
        if (player != CurrentPlayer)
        {
            throw new Exception("不是該玩家的回合");
        }
    }


    #endregion Private Functions

    /// <summary>
    /// 購買土地
    /// </summary>
    /// <param name="playerId">購買玩家ID</param>
    /// <param name="BlockId">購買土地ID</param>
    public void BuyLand(string playerId, string BlockId)
    {
        Player player = GetPlayer(playerId);
        AddDomainEvent(player.BuyLand(_map, BlockId));
    }
}