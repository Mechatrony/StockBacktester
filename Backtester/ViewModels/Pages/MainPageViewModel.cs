using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Backtester.Crypto;
using Backtester.Crypto.Exchange;
using Backtester.Crypto.Strategies;
using Backtester.Models;
using Backtester.Services;
using Backtester.Utils;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using Backtester.ViewModels.UserControls;

namespace Backtester.ViewModels.Pages;

public partial class MainPageViewModel : ObservableObject
{
    private readonly FileService fileService;
    private readonly ExchangeService exchangeService;
    private readonly BacktestService backtestService;
    private readonly BacktestResultViewModel backtestResultViewModel = new();

    [ObservableProperty]
    public partial StrategyBase SelectedStrategy { get; set; }
    [ObservableProperty]
    public partial ObservableCollection<StrategyBase> Strategies { get; set; } = [];
    [ObservableProperty]
    public partial ObservableCollection<CoinExchange> Exchanges { get; set; } =
    [
        CoinExchange.BinanceFutures,
        CoinExchange.BinanceSpot
    ];
    [ObservableProperty]
    public partial CoinExchange SelectedExchange { get; set; } = CoinExchange.BinanceFutures;

    [ObservableProperty]
    public partial ObservableCollection<StrategyParameterViewModel> MajorParameters { get; set; } = new();
    [ObservableProperty]
    public partial ObservableCollection<StrategyParameterViewModel> MinorParameters { get; set; } = new();
    [ObservableProperty]
    public partial ObservableCollection<StrategyParameterViewModel> StrategyParameters { get; set; } = new();

    // MajorParameters
    [ObservableProperty]
    public partial string TargetCoins { get; set; } = "BTC";
    [ObservableProperty]
    public partial Timeframe Timeframe { get; set; } = Timeframe.EightHours;

    public List<Timeframe> Timeframes = new List<Timeframe>()
    {
        Timeframe.OneMinute,
        Timeframe.ThreeMinutes,
        Timeframe.FiveMinutes,
        Timeframe.FifteenMinutes,
        Timeframe.ThirtyMinutes,
        Timeframe.OneHour,
        Timeframe.TwoHours,
        Timeframe.FourHours,
        Timeframe.SixHours,
        Timeframe.EightHours,
        Timeframe.TwelveHours,
        Timeframe.OneDay,
        Timeframe.OneWeek,
    };

    public MainPageViewModel(
        FileService fileService,
        ExchangeService exchangeService,
        BacktestService backtestService)
    {
        this.fileService = fileService;
        this.exchangeService = exchangeService;
        this.backtestService = backtestService;

        this.exchangeService.Register<BinanceClient>();
        this.exchangeService.Exchange = CoinExchange.BinanceFutures;

        PropertyChanged += MainPageViewModel_PropertyChanged;

        //Strategies.Add(new ReverseDca(backtestService, exchangeService));
        //Strategies.Add(new VolatilityBreakout(backtestService, exchangeService));
        //Strategies.Add(new DualMomentum(backtestService, exchangeService));
        Strategies.Add(new FourHmaStrategy(backtestService, exchangeService));
        SelectedStrategy = Strategies.First();
    }

    private void MainPageViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(SelectedStrategy):
                ShowParameters();
                break;
            case nameof(SelectedExchange):
                switch (SelectedExchange)
                {
                    case CoinExchange.BinanceFutures:
                        exchangeService.Exchange = SelectedExchange;
                        exchangeService.ExchangeClient.MarketType = MarketType.Futures;
                        GetParameter("MarketFeeRate")?.SetValue(0.0005d);
                        GetParameter("LimitFeeRate")?.SetValue(0.0002d);
                        break;
                    case CoinExchange.BinanceSpot:
                        exchangeService.Exchange = SelectedExchange;
                        exchangeService.ExchangeClient.MarketType = MarketType.Spot;
                        GetParameter("MarketFeeRate")?.SetValue(0.001d);
                        GetParameter("LimitFeeRate")?.SetValue(0.001d);
                        break;
                }
                break;
            case nameof(TargetCoins):
                if (SelectedStrategy != null)
                    SelectedStrategy.TargetCoins = TargetCoins;
                break;
            case nameof(Timeframe):
                if (SelectedStrategy != null)
                    SelectedStrategy.Timeframe = Timeframe;
                break;
        }
    }

    private StrategyParameterValueViewModel? GetParameter(string name)
    {
        ObservableCollection<StrategyParameterViewModel>[] collections =
            [ MajorParameters, MinorParameters, StrategyParameters ];
        foreach (var collection in collections)
        {
            foreach (var parameter in collection)
            {
                if (parameter.Value.Name == name)
                {
                    return parameter.Value;
                }
            }
        }
        return null;
    }

    private void ShowParameters()
    {
        TargetCoins = SelectedStrategy.TargetCoins;
        Timeframe = SelectedStrategy.Timeframe;

        MajorParameters = SelectedStrategy.MajorParameters.ToObservableCollection();
        MinorParameters = SelectedStrategy.MinorParameters.ToObservableCollection();
        StrategyParameters = SelectedStrategy.StrategyParameters.ToObservableCollection();
    }

    private void ApplyParameters()
    {
        SelectedStrategy.TargetCoins = TargetCoins;
        SelectedStrategy.TargetCoinNames = TargetCoins.Split(',');
        SelectedStrategy.Timeframe = Timeframe;

        SelectedStrategy.MajorParameters = MajorParameters.ToList();
        SelectedStrategy.MinorParameters = MinorParameters.ToList();
        SelectedStrategy.StrategyParameters = StrategyParameters.ToList();
    }

    private void ShowResult()
    {
        if (backtestService.BacktestDetails.Count == 0) return;

        ShowDetails();
        ShowSummary();

        Logger.Log("Backtest complete", LogLevel.Warn);
    }

    private void ShowDetails()
    {
        if (backtestService.BacktestDetails.Count == 0)
            return;

        backtestResultViewModel.Charts.Clear();

        var realStartTime = backtestService.BacktestDetails.First().DateTime;
        var realEndTime = backtestService.BacktestDetails.Last().DateTime;

        // BTC chart
        var btcOhlcvSeries = backtestService.Ohlcvs["BTC"]
            .SkipWhile(kvp => !kvp.DateTime.InRange(realStartTime, realEndTime))
            .ToList();
        FinancialChartViewModel btcChartViewModel
            = new FinancialChartViewModel("BTC", btcOhlcvSeries);

        if (backtestService.Indicators.TryGetValue(
            "BTC", out Dictionary<string, Dictionary<DateTime, double>>? btcIndicators))
        {
            for (int indicatorIndex = 0; indicatorIndex < 4; ++indicatorIndex)
            {
                IndicatorViewModel indicator = new IndicatorViewModel("BTC");
                if (indicatorIndex < btcIndicators.Count)
                {
                    indicator.IndicatorName = btcIndicators.ElementAt(indicatorIndex).Key;
                    indicator.Data = btcIndicators.ElementAt(indicatorIndex).Value
                        .SkipWhile(kvp => !kvp.Key.InRange(realStartTime, realEndTime))
                        .ToDictionary();
                }
                btcChartViewModel.Indicators.Add(indicator);
            }
        }

        backtestResultViewModel.Charts.Add(btcChartViewModel);

        // Other charts
        foreach (var ohlcvPair in backtestService.Ohlcvs)
        {
            string coinName = ohlcvPair.Key;
            if (coinName == "BTC") continue;

            List<Ohlcv> ohlcvSeries = ohlcvPair.Value
                .SkipWhile(kvp => !kvp.DateTime.InRange(realStartTime, realEndTime))
                .ToList();
            FinancialChartViewModel financialChartViewModel = new FinancialChartViewModel(coinName, ohlcvSeries);

            if (backtestService.Indicators.TryGetValue(
                coinName, out Dictionary<string, Dictionary<DateTime, double>>? indicators))
            {
                for (int indicatorIndex = 0; indicatorIndex < 4; ++indicatorIndex)
                {
                    IndicatorViewModel indicator = new IndicatorViewModel(coinName);
                    if (indicatorIndex < indicators.Count)
                    {
                        indicator.IndicatorName = indicators.ElementAt(indicatorIndex).Key;
                        indicator.Data = indicators.ElementAt(indicatorIndex).Value
                            .SkipWhile(kvp => !kvp.Key.InRange(realStartTime, realEndTime))
                            .ToDictionary();
                    }
                    financialChartViewModel.Indicators.Add(indicator);
                }
            }

            backtestResultViewModel.Charts.Add(financialChartViewModel);
        }

        backtestResultViewModel.BacktestDetails
            = new ObservableCollection<BacktestStatus>(backtestService.BacktestDetails);

        // BTC RoR %
        var btcRorSeries = new Dictionary<DateTime, double>(backtestService.BacktestDetails.Count);
        var btcOhlcvs = backtestService.Ohlcvs["BTC"];
        int btcIndex = btcOhlcvs.FindIndex(x => x.DateTime == realStartTime);
        double initBtcPrice = btcOhlcvs[btcIndex].OpenPrice;
        foreach (var detail in backtestResultViewModel.BacktestDetails)
        {
            DateTime dateTime = detail.DateTime;
            double btcRor = (btcOhlcvs[btcIndex].ClosePrice / initBtcPrice - 1d) * 100d;
            btcRorSeries[dateTime] = btcRor;
            ++btcIndex;
        }
        backtestResultViewModel.BtcRorSeries = btcRorSeries;
    }

    private void ShowSummary()
    {
        if (backtestService.BacktestDetails.Count == 0)
            return;

        // TODO: Fix sharpe, sortino, pnl ratio
        double ror = (backtestService.TotalBalance - backtestService.InitialBalance)
            / backtestService.InitialBalance;

        // Sharpe and Sortino
        List<double> yieldSeries = new List<double>();
        foreach (var tradeResults in backtestService.TradeResults.Values)
        {
            foreach (var trade in tradeResults)
            {
                yieldSeries.Add(trade.Yield / 100d);
            }
        }
        IEnumerable<double> lossYieldSeries = yieldSeries.Where(x => x.IsLessThan(0));

        int investingDays = (int)
            (backtestService.BacktestDetails.Last().DateTime
                - backtestService.BacktestDetails.First().DateTime)
            .TotalDays;
        double riskFreeCagr = 0.05;
        double cagr = MathUtils.CAGR(investingDays, ror);
        double annualStdev = MathUtils.StandardDeviation(yieldSeries)
            * Math.Sqrt(yieldSeries.Count);
        double negativeAnnualStdev = MathUtils.StandardDeviation(lossYieldSeries)
            * Math.Sqrt(lossYieldSeries.Count());
        double sharpe = (cagr - riskFreeCagr) / annualStdev;
        double sortino = (cagr - riskFreeCagr) / negativeAnnualStdev;

        // PNL Ratio and Kelly
        double profitRateAvg = yieldSeries.Where(y => y.IsGreaterThan(0)).AverageIfValid();
        double lossRateAvg = lossYieldSeries.AverageIfValid();
        double? pnlRatio = profitRateAvg / -lossRateAvg;
        double? winRate = backtestService.BacktestDetails.LastOrDefault()?.WinRate;
        double? kelly = winRate - (1d - winRate) / pnlRatio;

        backtestResultViewModel.Ror = ror * 100d;
        backtestResultViewModel.Cagr = cagr * 100d;
        backtestResultViewModel.Mdd = backtestService.Mdd;
        backtestResultViewModel.WinRate = winRate.GetValueOrDefault();
        backtestResultViewModel.WinCount = backtestService.WinCount;
        backtestResultViewModel.TradeCount = backtestService.WinCount + backtestService.LoseCount;
        backtestResultViewModel.Sharpe = sharpe;
        backtestResultViewModel.Sortino = sortino;
        backtestResultViewModel.PnlRatio = pnlRatio.Value;
        backtestResultViewModel.Kelly = kelly.GetValueOrDefault();
        backtestResultViewModel.BtcRor = backtestResultViewModel.BtcRorSeries.Last().Value;
    }

    [RelayCommand]
    private async Task RunBacktest()
    {
        Logger.Log("Run Backtest");

        try
        {
            ApplyParameters();
            await SelectedStrategy.Run();
            ShowResult();
        }
        catch (Exception ex)
        {
            Logger.Log(ex.ToString(), LogLevel.Error);
        }
    }

    [RelayCommand]
    private async Task RunRelayBacktest()
    {
        if (backtestResultViewModel.BacktestDetails.Count == 0)
            return;

        string[] names = TargetCoins.ToUpper().Split(',');
        //string[] names = exchangeService.ExchangeClient.FetchAllCoinInfoAsync().Result.Keys.ToArray();
        ApplyParameters();
        DataTable dataTable = new DataTable();
        dataTable.Columns.Add("Name");
        dataTable.Columns.Add("RoR");
        dataTable.Columns.Add("MDD");
        dataTable.Columns.Add("WinRate");
        dataTable.Columns.Add("PnlRatio");
        dataTable.Columns.Add("Sharpe");
        dataTable.Columns.Add("Sortino");
        dataTable.Columns.Add("Kelly");
        dataTable.Columns.Add("StartDate");
        dataTable.Columns.Add("EndDate");

        foreach (string name in names)
        {
            SelectedStrategy.TargetCoins = name;

            if (!await SelectedStrategy.Run())
                continue;

            ShowResult();

            string startDate = backtestResultViewModel
                .BacktestDetails.First()
                .DateTime.ToString("yy-MM-dd");
            string endDate = backtestResultViewModel
                .BacktestDetails.Last()
                .DateTime.ToString("yy-MM-dd");

            dataTable.Rows.Add(
                name,
                Math.Round(backtestResultViewModel.Ror, 2),
                Math.Round(backtestResultViewModel.Mdd, 2),
                Math.Round(backtestResultViewModel.WinRate, 2),
                Math.Round(backtestResultViewModel.PnlRatio, 2),
                Math.Round(backtestResultViewModel.Sharpe, 2),
                Math.Round(backtestResultViewModel.Sortino, 2),
                Math.Round(backtestResultViewModel.Kelly, 2),
                startDate,
                endDate
            );
        }

        SaveFileDialog saveFileDialog = new SaveFileDialog
        {
            Filter = "csv files (.csv)|.csv"
        };
        if (saveFileDialog.ShowDialog().GetValueOrDefault())
        {
            fileService.SaveCsv(saveFileDialog.FileName, dataTable);
        }
    }

    [RelayCommand]
    private async Task Test()
    {
        Logger.Log("Hello");
        await Task.CompletedTask;
    }
}
