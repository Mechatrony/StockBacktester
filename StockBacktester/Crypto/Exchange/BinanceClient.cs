using Binance.Net.Clients;
using Binance.Net.Enums;
using Binance.Net.Interfaces;
using StockBacktester.Models;
using StockBacktester.Services;
using StockBacktester.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockBacktester.Crypto.Exchange;

public class BinanceClient : IExchangeClient {
  public BinanceClient() {
    restClient = new BinanceRestClient();
    restClient.ClientOptions.OutputOriginalData = true;
    restClient.ClientOptions.RateLimitingBehaviour = global::CryptoExchange.Net.Objects.RateLimitingBehaviour.Wait;
    socketClient = new BinanceSocketClient();
  }

  private FileService? fileService;
  private static readonly int timeDifference = 9;
  private bool isBusy = false;

  private readonly BinanceRestClient restClient;
  private readonly BinanceSocketClient socketClient;

  public string? ApiKey { get; set; }
  public string? ApiSecret { get; set; }
  public int RequestDelay { get; set; } = 200;
  public string DbPath { get; set; }
    = Path.Combine(GlobalVariables.DbRootPath, "Binance", MarketType.Futures.ToString());

  private MarketType marketType = MarketType.Futures;
  public MarketType MarketType {
    get => marketType;
    set {
      marketType = value;
      DbPath = Path.Combine(
        GlobalVariables.DbRootPath,
        "Binance",
        marketType.ToString()
      );
    }
  }

  public async Task<Dictionary<string, List<Ohlcv>>> LoadAllOhlcvAsync(
    Dictionary<string, CoinInfo> allCoinInfo, Timeframe timeframe,
    DateTime startTime, DateTime endTime, DateTime shouldListedBefore) {

    var allOhlcvs = new Dictionary<string, List<Ohlcv>>();
    var tasks = new List<Task>();
    foreach (var kvp in allCoinInfo) {
      if (endTime <= kvp.Value.ListingDate) continue;
      if (shouldListedBefore < kvp.Value.ListingDate) continue;

      Task task = Task.Run(
        async () => {
          string name = kvp.Key;
          var ohlcvs = await LoadOhlcvAsync(name, timeframe, startTime, endTime);
          try {
            if (ohlcvs.Count > 0) {
              allOhlcvs[name] = ohlcvs;
              Logger.Log($"Loaded {ohlcvs.Count} {name} candles");
            }
          } catch (Exception ex) {
            Logger.Log($"Name: {name}", LogLevel.Error);
            Logger.Log(ex.ToString(), LogLevel.Error);
          }
        }
      );
      tasks.Add(task);
    }

    await Task.WhenAll(tasks);

    return allOhlcvs;
  }

  public async Task<List<Ohlcv>> LoadOhlcvAsync(
    string name, Timeframe timeframe, DateTime startTime, DateTime endTime) {
    List<Ohlcv> ohlcvs = new();
    List<Ohlcv> dbData = null;

    TimeSpan timeInterval = timeframe.ToTimeSpan();
    endTime = UserMath.Min(endTime, DateTime.Now - timeInterval);

    // Load from DB
    string folderPath = Path.Combine(DbPath, timeframe.ToStringExpression());
    string fileName = $"{name}.csv";
    string fileFullName = Path.Combine(folderPath, fileName);
    bool updateRequired = false;
    if (File.Exists(fileFullName)) {
      fileService ??= App.GetService<FileService>();
      dbData = fileService.GetOhlcvs(fileFullName);
      for (int dbIndex = 0; dbIndex < dbData.Count; ++dbIndex) {
        Ohlcv ohlcv = dbData.ElementAt(dbIndex);
        if (!ohlcv.DateTime.InRange(startTime, endTime)) continue;
        if (ohlcv.DateTime == ohlcvs.LastOrDefault()?.DateTime) continue;
        ohlcvs.Add(ohlcv);

        // Fetch pending parts (if not continuous)
        if (dbIndex == dbData.Count - 1) break;
        DateTime nextDateTimeExpected = ohlcv.DateTime + timeInterval;
        DateTime nextDateTimeInDb = dbData.ElementAt(dbIndex + 1).DateTime;
        if (nextDateTimeExpected != nextDateTimeInDb) {
          if (nextDateTimeInDb == ohlcv.DateTime) continue;

          var pendingOhlcvs = await FetchOhlcvAsync(
            name, timeframe, nextDateTimeExpected, nextDateTimeInDb);
          ohlcvs.AddRange(pendingOhlcvs);
          updateRequired = true;
        }
      }
    }

    // Fetch pending parts
    if (ohlcvs.Count == 0) {
      if (dbData == null
        || !dbData.First().IsFirst
        || endTime > dbData.First().DateTime) {
        ohlcvs = await FetchOhlcvAsync(name, timeframe, startTime, endTime);
        updateRequired = true;
      }
    } else {
      DateTime firstDbTime = ohlcvs.First().DateTime;
      if (!ohlcvs.First().IsFirst
        && startTime <= firstDbTime - timeInterval) {
        var prevOhlcvs = await FetchOhlcvAsync(
          name, timeframe, startTime, firstDbTime - timeInterval);
        if (prevOhlcvs.Count > 0) {
          prevOhlcvs.AddRange(ohlcvs);
          ohlcvs = prevOhlcvs;
          updateRequired = true;
        } else if (!ohlcvs.First().IsFirst) {
          ohlcvs.First().IsFirst = true;
          updateRequired = true;
        }
      }

      DateTime lastDbTime = ohlcvs.Last().DateTime;
      if (endTime >= lastDbTime + 2 * timeInterval) {
        var nextOhlcvs = await FetchOhlcvAsync(
          name, timeframe, lastDbTime + timeInterval, endTime);
        if (nextOhlcvs.Count > 0) {
          ohlcvs.AddRange(nextOhlcvs);
          updateRequired = true;
        }
      }

      if (updateRequired) {
        ohlcvs = [.. ohlcvs.OrderBy(ohlcv => ohlcv.DateTime)];
      }
    }

    // Update DB
    if (updateRequired && ohlcvs.Count > 0) {
      fileService ??= App.GetService<FileService>();
      dbData ??= new List<Ohlcv>(ohlcvs.Count);
      dbData.AddRange(ohlcvs);
      dbData = [.. dbData.OrderBy(ohlcv => ohlcv.DateTime)];
      fileService.SaveOhlcvs(folderPath, fileName, dbData);
    }

    return ohlcvs;
  }

  public async Task<List<Ohlcv>> FetchOhlcvAsync(
    string name, Timeframe timeframe, DateTime startTime, DateTime endTime) {

    while (isBusy) {
      await Task.Delay(10);
    }
    isBusy = true;

    List<Ohlcv> combinedOhlcvs = new();
    DateTime dateTime = startTime.AddHours(-timeDifference);
    TimeSpan timeInterval = timeframe.ToTimeSpan();
    string symbol = name + "USDT";
    bool complete = false;
    while (!complete) {
      var apiResult = MarketType switch {
        MarketType.Spot => await restClient.SpotApi.ExchangeData.GetKlinesAsync(
          symbol, (KlineInterval)timeframe, dateTime, limit: 1500),
        MarketType.Futures => await restClient.UsdFuturesApi.ExchangeData.GetKlinesAsync(
          symbol, (KlineInterval)timeframe, dateTime, limit: 1500),
        _ => throw new Exception()
      };

      await Task.Delay(RequestDelay);
      if (!apiResult.Success) {
        throw new Exception(apiResult.Error.Message);
      }
      IEnumerable<IBinanceKline> klines = apiResult.Data;
      if (klines == null || !klines.Any()) {
        break;
      }

      combinedOhlcvs.EnsureCapacity(combinedOhlcvs.Count + klines.Count());
      List<Ohlcv> ohlcvs = KlinesToOhlcvs(klines);
      int outIndex = ohlcvs.FindIndex(ohlcv => !ohlcv.DateTime.InRange(startTime, endTime));
      if (outIndex == -1) {
        foreach (Ohlcv ohlcv in ohlcvs) {
          combinedOhlcvs.Add(ohlcv);
        }
        dateTime = ohlcvs.Last().DateTime.AddHours(-timeDifference) + timeInterval;
      } else {
        foreach (Ohlcv ohlcv in ohlcvs.Take(outIndex)) {
          combinedOhlcvs.Add(ohlcv);
        }
        complete = true;
      }
    }
    isBusy = false;

    if (combinedOhlcvs.Count > 0) {
      Ohlcv firstOhlcv = combinedOhlcvs.First();
      if (startTime.AddHours(-timeDifference) + 2 * timeInterval <= firstOhlcv?.DateTime) {
        firstOhlcv.IsFirst = true;
      }
    }

    return combinedOhlcvs;
  }

  private static List<Ohlcv> KlinesToOhlcvs(IEnumerable<IBinanceKline> klines) {
    List<Ohlcv> ohlcModels = new(klines.Count());
    for (int index = 0; index < klines.Count(); ++index) {
      ohlcModels.Add(new Ohlcv(
        klines.ElementAt(index).OpenTime.AddHours(timeDifference),
        (double)klines.ElementAt(index).OpenPrice,
        (double)klines.ElementAt(index).HighPrice,
        (double)klines.ElementAt(index).LowPrice,
        (double)klines.ElementAt(index).ClosePrice,
        (double)klines.ElementAt(index).Volume
      ));
    }
    return ohlcModels;
  }

  public async Task<Dictionary<string, CoinInfo>> FetchAllCoinInfoAsync() {
    while (isBusy) {
      await Task.Delay(10);
    }
    isBusy = true;

    Dictionary<string, CoinInfo> coinInfos;

    if (MarketType == MarketType.Futures) {
      var exchangeInfoApiResult
        = await restClient.UsdFuturesApi.ExchangeData.GetExchangeInfoAsync();
      if (!exchangeInfoApiResult.Success) {
        isBusy = false;
        throw new Exception(exchangeInfoApiResult.Error.Message);
      }
      await Task.Delay(RequestDelay);

      var symbols = exchangeInfoApiResult.Data.Symbols;
      coinInfos = new(symbols.Count());
      foreach (var symbol in symbols) {
        if (!symbol.Name.EndsWith("USDT")) continue;

        string name = symbol.Name[..symbol.Name.IndexOf("USDT")];
        coinInfos.Add(name, new CoinInfo(
          name, symbol.QuantityPrecision, symbol.PricePrecision, symbol.ListingDate));
      }
    } else if (MarketType == MarketType.Spot) {
      var apiResult = await restClient.SpotApi.ExchangeData.GetExchangeInfoAsync();
      if (!apiResult.Success) {
        isBusy = false;
        throw new Exception(apiResult.Error.Message);
      }
      await Task.Delay(RequestDelay);

      var symbols = apiResult.Data.Symbols;
      coinInfos = new(symbols.Count());
      foreach (var symbol in symbols) {
        if (!symbol.Name.EndsWith("USDT")) continue;

        string name = symbol.Name[..symbol.Name.IndexOf("USDT")];
        coinInfos.Add(name, new CoinInfo(
          name, symbol.BaseAssetPrecision, symbol.QuoteAssetPrecision, new DateTime()));
      }
    } else {
      isBusy = false;
      throw new Exception();
    }

    // TODO: Find any other way to get circulating supply
    // Circulating Supply
    //var productsApiResult = await restClient.SpotApi.ExchangeData.GetProductsAsync();
    //if (!productsApiResult.Success) {
    //  isBusy = false;
    //  throw new Exception(productsApiResult.Error.Message);
    //}
    //await Task.Delay(RequestDelay);
    //var products = productsApiResult.Data;
    //foreach (var product in products) {
    //  if (!product.Symbol.EndsWith("USDT")) continue;
    //  string name = product.Symbol[..product.Symbol.IndexOf("USDT")];
    //  if (!coinInfos.TryGetValue(name, out CoinInfo coinInfo)) continue;

    //  coinInfo.CirculatingSupply = (double)product.CirculatingSupply;
    //}

    isBusy = false;

    return coinInfos;
  }

  public async Task TestFuncAsync() {
    await Task.CompletedTask;
  }
}

