namespace StockBacktester.Crypto;

public class CompletedTradeResult {
  public string CoinName { get; set; }
  public DateTime DateTime { get; set; }
  public PositionSide PositionSide { get; set; }
  public double EntryPrice { get; set; }
  public double ClosePrice { get; set; }
  public double Pnl { get; set; }
  public double Yield { get; set; }
}
