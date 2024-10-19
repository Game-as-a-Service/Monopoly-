namespace Monopoly.ApplicationLayer.Application.MonopolyUsecases.Queries.Interfaces;

/// <summary>
/// 用來確認遊戲是否存在的查詢介面
/// </summary>
public interface IGameExistenceInquiry
{
    /// <summary>
    /// 確認遊戲是否存在
    /// </summary>
    /// <param name="gameId">遊戲 Id</param>
    /// <returns>當遊戲存在則回傳 true</returns>
    Task<bool> CheckGameExistenceAsync(string gameId);
}