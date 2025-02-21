namespace Backtester.Crypto.Exchange;

public abstract class ExchangeClient
{
    public string? ApiKey { get; }
    public string? ApiSecret { get; }
    public int RequestDelay { get; set; } = 200;
    public abstract string DbPath { get; set; }
    public virtual MarketType MarketType { get; set; }

    public abstract Task<List<Ohlcv>> FetchOhlcvAsync(
        string symbol, Timeframe timeframe, DateTime startTime, DateTime endTime);
    public abstract Task<List<Ohlcv>> LoadOhlcvAsync(
        string symbol, Timeframe timeframe, DateTime startTime, DateTime endTime);
    public abstract Task<Dictionary<string, List<Ohlcv>>> LoadAllOhlcvAsync(
          Dictionary<string, CoinInfo> allCoinInfo, Timeframe timeframe,
          DateTime startTime, DateTime endTime, DateTime shouldListedBefore);
    public abstract Task<Dictionary<string, CoinInfo>> FetchAllCoinInfoAsync();
    public abstract Task TestFuncAsync();
}