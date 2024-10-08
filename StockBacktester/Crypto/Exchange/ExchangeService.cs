namespace StockBacktester.Crypto.Exchange;

public class ExchangeService {
  public CryptoExchange Exchange { get; set; }
  private readonly Dictionary<CryptoExchange, IExchangeClient> exchangeClients = [];

  public IExchangeClient? ExchangeClient {
    get {
      if (exchangeClients.TryGetValue(Exchange, out IExchangeClient? exchangeClient)) {
        return exchangeClient;
      }
      return null;
    }
  }

  public void Register<T>() where T : IExchangeClient {
    if (typeof(T) == typeof(BinanceClient)) {
      var binanceClient = new BinanceClient();
      exchangeClients.TryAdd(CryptoExchange.BinanceFutures, binanceClient);
      exchangeClients.TryAdd(CryptoExchange.BinanceSpot, binanceClient);
    }
  }

  public async Task<List<Ohlcv>> LoadOhlcvAsync(
    string name, Timeframe timeframe, DateTime startTime, DateTime endTime) {
    return await ExchangeClient
      .LoadOhlcvAsync(name, timeframe, startTime, endTime)
      .ConfigureAwait(false);
  }

  public async Task<Dictionary<string, List<Ohlcv>>> LoadAllOhlcvAsync(
    Dictionary<string, CoinInfo> allCoinInfo, Timeframe timeframe,
    DateTime startTime, DateTime endTime, DateTime shouldListedBefore) {
    return await ExchangeClient
      .LoadAllOhlcvAsync(allCoinInfo, timeframe, startTime, endTime, shouldListedBefore)
      .ConfigureAwait(false);
  }

  public async Task<Dictionary<string, CoinInfo>> FetchAllCoinInfoAsync() {
    return await ExchangeClient.FetchAllCoinInfoAsync();
  }
}
