namespace StockBacktester.Crypto.Exchange;

public interface IExchangeClient {
  public string? ApiKey { get; }
  public string? ApiSecret { get; }
  public int RequestDelay { get; set; }
  public string DbPath { get; set; }

  public Task<List<Ohlcv>> FetchOhlcvAsync(
    string symbol, Timeframe timeframe, DateTime startTime, DateTime endTime);
  public Task<List<Ohlcv>> LoadOhlcvAsync(
    string symbol, Timeframe timeframe, DateTime startTime, DateTime endTime);
  public Task<Dictionary<string, List<Ohlcv>>> LoadAllOhlcvAsync(
    Dictionary<string, CoinInfo> allCoinInfo, Timeframe timeframe,
    DateTime startTime, DateTime endTime, DateTime shouldListedBefore);
  public Task<Dictionary<string, CoinInfo>> FetchAllCoinInfoAsync();
  public Task TestFuncAsync();
}