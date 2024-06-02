using Monopoly.DomainLayer.Domain.Interfaces;

namespace Monopoly.DomainLayer.Domain;

public class Dice : IDice
{
    public int Value { get; private set; }

    /// <summary>
    /// 將 Value 設為 1 ~ 6 的數字
    /// </summary>
    public void Roll()
    {
        Random random = new();
        Value = random.Next(1, 6);
    }
}