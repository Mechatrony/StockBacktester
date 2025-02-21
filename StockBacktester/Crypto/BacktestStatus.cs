namespace Backtester.Crypto;

public class BacktestStatus
{
    public DateTime DateTime { get; set; }
    public List<Coin> TradedCoins { get; set; } = new();
    public double TotalBalance { get; set; } = 0d;
    public double FreeBalance { get; set; } = 0d;
    public double MaximumBalance { get; set; } = 0d;
    public double MinimumBalance { get; set; } = 0d;
    public double TotalYield { get; set; } = 0d;
    public double Yield { get; set; } = 0d;
    public double Pnl { get; set; } = 0d;
    public double Drawdown { get; set; } = 0d;
    public double Mdd { get; set; } = 0d;
    public int WinCount { get; set; } = 0;
    public int LoseCount { get; set; } = 0;
    public double WinRate { get; set; } = 0d;
}
