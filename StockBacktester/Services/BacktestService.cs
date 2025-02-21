using Backtester.Crypto;
using Backtester.Crypto.Strategies;

namespace Backtester.Services;

public class BacktestService
{
    public StrategyBase Strategy { get; set; }
    public Dictionary<string, Coin> Coins { get; set; } = new();

    public Dictionary<string, List<Ohlcv>> Ohlcvs { get; set; } = new();
    public Dictionary<string, Dictionary<DateTime, double>> OpenPrices { get; set; } = new();
    public Dictionary<string, Dictionary<DateTime, double>> HighPrices { get; set; } = new();
    public Dictionary<string, Dictionary<DateTime, double>> LowPrices { get; set; } = new();
    public Dictionary<string, Dictionary<DateTime, double>> ClosePrices { get; set; } = new();
    public Dictionary<string, Dictionary<DateTime, double>> Volumes { get; set; } = new();

    public Dictionary<string, Dictionary<string, Dictionary<DateTime, double>>> Indicators { get; set; } = new();
    public List<BacktestStatus> BacktestDetails { get; set; } = new();
    public Dictionary<string, List<CompletedTradeResult>> TradeResults { get; set; } = new();
    public Dictionary<string, List<OrderInfo>> Orders { get; set; } = new();

    public double InitialBalance { get; set; } = 10000d;
    public double TotalBalance { get; set; } = 10000d;
    public double FreeBalance { get; set; } = 10000d;
    public double MaximumBalance { get; set; } = 10000d;
    public double MinimumBalance { get; set; } = 10000d;
    public double Mdd { get; set; } = 0d;
    public int WinCount { get; set; } = 0;
    public int LoseCount { get; set; } = 0;

    public void Initialize(double initialBalance)
    {
        Coins.Clear();
        Ohlcvs.Clear();
        Indicators.Clear();
        BacktestDetails.Clear();

        InitialBalance = initialBalance;
        TotalBalance = initialBalance;
        FreeBalance = initialBalance;
        MaximumBalance = initialBalance;
        MinimumBalance = initialBalance;
        Mdd = 0d;
        WinCount = 0;
        LoseCount = 0;
    }
}
