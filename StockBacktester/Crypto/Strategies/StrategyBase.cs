using StockBacktester.Crypto.Exchange;
using StockBacktester.Enums;
using StockBacktester.Models;
using StockBacktester.Services;
using StockBacktester.Utils;
using StockBacktester.ViewModels;

namespace StockBacktester.Crypto.Strategies;

public abstract class StrategyBase
{
    protected readonly BacktestService backtestService;
    protected readonly ExchangeService exchangeService;

    protected const double OVER_HUNDRED = 0.01d;

    protected List<Ohlcv> btcOhlcvSeries;
    protected int prepareCandles = 1;
    protected Dictionary<string, CoinInfo> allCoinInfo;
    protected Dictionary<string, int> ohlcvIndexes = new();
    protected TimeSpan timeInterval;
    protected DateTime loadStartDate;
    protected DateTime firstTradeTime;
    protected int firstTradeIndex = -1;

    public string Title { get; set; } = string.Empty;
    public string[] TargetCoinNames { get; set; } = [];
    public DateTime EndDate { get; set; } = DateTime.Now.Date;

    public List<StrategyParameterViewModel> MajorParameters { get; set; } = new();
    public List<StrategyParameterViewModel> MinorParameters { get; set; } = new();
    public List<StrategyParameterViewModel> StrategyParameters { get; set; } = new();

    public double OrderRatio { get; set; } = 0.98f;
    public double MinimumOrderValuation { get; set; } = 5f;
    public double LimitFeeRate { get; set; } = 0.02d * OVER_HUNDRED;
    public double MarketFeeRate { get; set; } = 0.05d * OVER_HUNDRED;

    public string TargetCoins { get; set; } = string.Empty;
    public Timeframe Timeframe { get; set; } = Timeframe.OneDay;
    public DateTime StartDate { get; set; } = DateTime.Now.AddYears(-1).Date;
    public int TestDays { get; set; } = 365;

    public double InitialBalance { get; set; } = 10000d;
    public int Leverage { get; set; } = 1;
    public bool UseLong { get; set; } = true;
    public bool UseShort { get; set; } = false;

    public StrategyBase(BacktestService backtestService, ExchangeService exchangeService)
    {
        this.backtestService = backtestService;
        this.exchangeService = exchangeService;

        TargetCoins = "BTC";
        Timeframe = Timeframe.OneDay;

        //MajorParameters.Add(new StrategyParameterViewModel(nameof(TargetCoins), TargetCoins));
        //MajorParameters.Add(new StrategyParameterViewModel(nameof(Timeframe), Timeframe));
        MajorParameters.Add(new StrategyParameterViewModel(nameof(StartDate), "Start date", StartDate));
        MajorParameters.Add(new StrategyParameterViewModel(nameof(TestDays), "Test days", TestDays));
        MajorParameters.Add(new StrategyParameterViewModel(nameof(Leverage), Leverage));
        MajorParameters.Add(new StrategyParameterViewModel(nameof(UseLong), "Use long", UseLong));
        MajorParameters.Add(new StrategyParameterViewModel(nameof(UseShort), "Use short", UseShort));

        MinorParameters.Add(new StrategyParameterViewModel(nameof(InitialBalance), "Initial balance", InitialBalance));
        MinorParameters.Add(new StrategyParameterViewModel(nameof(MinimumOrderValuation), "Min. order valuation", MinimumOrderValuation));
        MinorParameters.Add(new StrategyParameterViewModel(nameof(OrderRatio), "Order ratio", OrderRatio));
        MinorParameters.Add(new StrategyParameterViewModel(nameof(LimitFeeRate), "Limit fee rate", LimitFeeRate, StrategyParameterType.DoublePercent));
        MinorParameters.Add(new StrategyParameterViewModel(nameof(MarketFeeRate), "Market fee rate", MarketFeeRate, StrategyParameterType.DoublePercent));
    }

    protected void ShowDefaultParameters()
    {
        GetParameter(nameof(StartDate))?.SetValue(StartDate);
        GetParameter(nameof(TestDays))?.SetValue(TestDays);
        GetParameter(nameof(Leverage))?.SetValue(Leverage);
        GetParameter(nameof(UseLong))?.SetValue(UseLong);
        GetParameter(nameof(UseShort))?.SetValue(UseShort);
        GetParameter(nameof(InitialBalance))?.SetValue(InitialBalance);
        GetParameter(nameof(MinimumOrderValuation))?.SetValue(MinimumOrderValuation);
        GetParameter(nameof(OrderRatio))?.SetValue(OrderRatio);
        GetParameter(nameof(LimitFeeRate))?.SetValue(LimitFeeRate);
        GetParameter(nameof(MarketFeeRate))?.SetValue(MarketFeeRate);
    }

    public StrategyParameterValueViewModel? GetParameter(string name)
    {
        List<StrategyParameterViewModel>[] parameterCollections =
        {
            MajorParameters, MinorParameters, StrategyParameters
        };

        foreach (var parameterCollection in parameterCollections)
        {
            foreach (var parameter in parameterCollection)
            {
                if (parameter.Value.Name == name)
                {
                    return parameter.Value;
                }
            }
        }

        return null;
    }

    protected virtual void LoadParameters()
    {
        foreach (var parameter in MajorParameters)
        {
            switch (parameter.Value.Name)
            {
                case nameof(StartDate):
                    StartDate = parameter.Value.DateTimeValue;
                    break;
                case nameof(TestDays):
                    TestDays = parameter.Value.IntValue;
                    break;
                case nameof(Leverage):
                    Leverage = parameter.Value.IntValue;
                    break;
                case nameof(UseLong):
                    UseLong = parameter.Value.BoolValue;
                    break;
                case nameof(UseShort):
                    UseShort = parameter.Value.BoolValue;
                    break;
            }
        }
        foreach (var parameter in MinorParameters)
        {
            switch (parameter.Value.Name)
            {
                case nameof(InitialBalance):
                    InitialBalance = parameter.Value.DoubleValue;
                    break;
                case nameof(OrderRatio):
                    OrderRatio = parameter.Value.DoubleValue;
                    break;
                case nameof(MinimumOrderValuation):
                    MinimumOrderValuation = parameter.Value.DoubleValue;
                    break;
                case nameof(LimitFeeRate):
                    LimitFeeRate = parameter.Value.DoublePercentValue;
                    break;
                case nameof(MarketFeeRate):
                    MarketFeeRate = parameter.Value.DoublePercentValue;
                    break;
            }
        }
        TargetCoinNames = TargetCoins.ToUpper().Split(',');
        EndDate = MathUtils.Min(StartDate.AddDays(TestDays), DateTime.Now);
    }

    protected virtual bool CheckParameters()
    {
        if (TargetCoinNames.Length == 0)
        {
            Logger.Log("TargetCoinNames is empty", LogLevel.Error);
            return false;
        }
        return true;
    }

    /// <summary>
    /// Initialize status, load parameters, load OHLCVs and create indicators.
    /// </summary>
    protected async Task<bool> DoInitProcessAsync()
    {
        backtestService.Initialize(InitialBalance);
        timeInterval = Timeframe.ToTimeSpan();

        LoadParameters();
        if (!CheckParameters()) return false;
        if (!await LoadCoinDataAsync()) return false;
        CreateIndicators();

        return true;
    }

    protected virtual async Task<bool> LoadCoinDataAsync()
    {
        backtestService.Coins.Clear();
        backtestService.Indicators.Clear();
        backtestService.TradeResults.Clear();
        backtestService.Orders.Clear();
        ohlcvIndexes.Clear();

        allCoinInfo = await exchangeService.FetchAllCoinInfoAsync();

        foreach (string name in TargetCoinNames)
        {
            if (!allCoinInfo.TryGetValue(name, out CoinInfo coinInfo))
            {
                Logger.Log($"'{name}' don't exist on {exchangeService.Exchange} market", LogLevel.Error);
                return false;
            }

            backtestService.Coins[name] = new Coin(coinInfo, Leverage);
            backtestService.Indicators[name] = new();
            backtestService.TradeResults[name] = new List<CompletedTradeResult>();
            backtestService.Orders[name] = new List<OrderInfo>();
            ohlcvIndexes[name] = -1;
        }

        return await LoadOhlcvAsync();
    }

    protected virtual async Task<bool> LoadOhlcvAsync()
    {
        loadStartDate = StartDate - timeInterval * prepareCandles;

        List<string> targetCoinNames = new List<string>() { "BTC" };
        foreach (string name in TargetCoinNames)
        {
            if (name == "BTC") continue;
            targetCoinNames.Add(name);
        }

        // Load Ohlcvs
        var tasks = new List<Task>();
        foreach (string name in targetCoinNames)
        {
            Task task = Task.Run(
        async () =>
        {
            var ohlcvSeries = await exchangeService.LoadOhlcvAsync(
            name, Timeframe, loadStartDate, EndDate);
            Logger.Log($"Loaded {ohlcvSeries.Count} {name} candles");
            backtestService.Ohlcvs[name] = ohlcvSeries;
            backtestService.OpenPrices[name] = ohlcvSeries
                .Select(ohlcv => new KeyValuePair<DateTime, double>(ohlcv.DateTime, ohlcv.OpenPrice))
                .ToDictionary();
            backtestService.HighPrices[name] = ohlcvSeries
                .Select(ohlcv => new KeyValuePair<DateTime, double>(ohlcv.DateTime, ohlcv.HighPrice))
                .ToDictionary();
            backtestService.LowPrices[name] = ohlcvSeries
                .Select(ohlcv => new KeyValuePair<DateTime, double>(ohlcv.DateTime, ohlcv.LowPrice))
                .ToDictionary();
            backtestService.ClosePrices[name] = ohlcvSeries
                .Select(ohlcv => new KeyValuePair<DateTime, double>(ohlcv.DateTime, ohlcv.ClosePrice))
                .ToDictionary();
            backtestService.Volumes[name] = ohlcvSeries
                .Select(ohlcv => new KeyValuePair<DateTime, double>(ohlcv.DateTime, ohlcv.Volume))
                .ToDictionary();
        }
      );
            tasks.Add(task);
        }
        await Task.WhenAll(tasks);

        // Sync Ohlcvs
        string newestName = backtestService.Ohlcvs.OrderBy(kvp => kvp.Value.Count).First().Key;
        var newestOhlcvList = backtestService.Ohlcvs[newestName];
        DateTime startTime = newestOhlcvList.First().DateTime;
        DateTime endTime = MathUtils.Min(
            newestOhlcvList.ElementAt(newestOhlcvList.Count - 1).DateTime,
            DateTime.Now - timeInterval);
        for (int index = 0; index < backtestService.Ohlcvs.Count; ++index)
        {
            string name = backtestService.Ohlcvs.ElementAt(index).Key;
            List<Ohlcv> ohlcvSeries = backtestService.Ohlcvs[name];
            backtestService.Ohlcvs[name] = ohlcvSeries
              .SkipWhile(ohlcv => !ohlcv.DateTime.InRange(startTime, endTime))
              .TakeWhile(ohlcv => ohlcv.DateTime.InRange(startTime, endTime))
              .ToList();
        }
        btcOhlcvSeries = backtestService.Ohlcvs["BTC"];

        firstTradeTime = startTime + timeInterval * prepareCandles;
        firstTradeIndex = prepareCandles;

        return true;
    }

    protected void SummarizeForCandle(BacktestStatus status)
    {
        status.TotalBalance = status.FreeBalance;
        foreach (Coin coin in backtestService.Coins.Values)
        {
            status.TotalBalance += coin.LongValuation - coin.LongMargin;
            status.TotalBalance += coin.ShortValuation - coin.ShortMargin;
        }

        status.MaximumBalance = Math.Max(status.MaximumBalance, status.TotalBalance);
        status.MinimumBalance = Math.Min(status.MinimumBalance, status.TotalBalance);
        status.Drawdown = Math.Min((status.TotalBalance - status.MaximumBalance) / status.MaximumBalance * 100d, 0);
        status.Mdd = Math.Min(status.Drawdown, status.Mdd);
        status.TotalYield = (status.TotalBalance - backtestService.InitialBalance) / backtestService.InitialBalance * 100d;
        status.Yield = (status.TotalBalance - backtestService.FreeBalance) / backtestService.FreeBalance;
        status.Pnl = status.TotalBalance - backtestService.FreeBalance;
        if (status.WinCount + status.LoseCount > 0)
        {
            status.WinRate = (double)status.WinCount / (status.WinCount + status.LoseCount);
        }

        backtestService.TotalBalance = status.TotalBalance;
        backtestService.FreeBalance = status.FreeBalance;
        backtestService.MaximumBalance = status.MaximumBalance;
        backtestService.MinimumBalance = status.MinimumBalance;
        backtestService.Mdd = status.Mdd;
        backtestService.WinCount = status.WinCount;
        backtestService.LoseCount = status.LoseCount;

        backtestService.BacktestDetails.Add(status);
    }

    protected BacktestStatus CreateBacktestStatus(DateTime indexTime)
    {
        return new BacktestStatus
        {
            DateTime = indexTime,
            TotalBalance = backtestService.TotalBalance,
            FreeBalance = backtestService.FreeBalance,
            MaximumBalance = backtestService.MaximumBalance,
            MinimumBalance = backtestService.MinimumBalance,
            Mdd = backtestService.Mdd,
            WinCount = backtestService.WinCount,
            LoseCount = backtestService.LoseCount,
        };
    }

    protected abstract void CreateIndicators();
    public abstract Task<bool> Run();
}
