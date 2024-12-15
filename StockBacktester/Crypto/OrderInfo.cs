namespace StockBacktester.Crypto;

public class OrderInfo
{
    public string CoinName { get; set; }
    public DateTime DateTime { get; set; }
    public PositionSide PositionSide { get; set; }
    public bool IsEntry { get; set; }
    public double Price { get; set; }
}
