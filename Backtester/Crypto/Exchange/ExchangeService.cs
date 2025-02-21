namespace Backtester.Crypto.Exchange;

public class ExchangeService
{
    private readonly Dictionary<CoinExchange, ExchangeClient> exchangeClients = [];
    public ExchangeClient ExchangeClient => exchangeClients[Exchange];

    public CoinExchange Exchange { get; set; }

    public void Register<T>() where T : ExchangeClient
    {
        if (typeof(T) == typeof(BinanceClient))
        {
            BinanceClient binanceClient = new BinanceClient();
            exchangeClients.TryAdd(CoinExchange.BinanceFutures, binanceClient);
            exchangeClients.TryAdd(CoinExchange.BinanceSpot, binanceClient);
        }
    }

    public async Task<List<Ohlcv>> LoadOhlcvAsync(
        string name, Timeframe timeframe, DateTime startTime, DateTime endTime)
    {
        return await ExchangeClient.LoadOhlcvAsync(name, timeframe, startTime, endTime);
    }

    public async Task<Dictionary<string, List<Ohlcv>>> LoadAllOhlcvAsync(
        Dictionary<string, CoinInfo> allCoinInfo, Timeframe timeframe,
        DateTime startTime, DateTime endTime, DateTime shouldListedBefore)
    {
        return await ExchangeClient.LoadAllOhlcvAsync(allCoinInfo, timeframe, startTime, endTime, shouldListedBefore);
    }

    public async Task<Dictionary<string, CoinInfo>> FetchAllCoinInfoAsync()
    {
        return await ExchangeClient.FetchAllCoinInfoAsync();
    }
}
