using StockBacktester.Crypto.Exchange;
using StockBacktester.Services;
using StockBacktester.Utils;
using StockBacktester.ViewModels;

namespace StockBacktester.Crypto.Strategies;

public class FourHmaStrategy : StrategyBase
{
    public bool UseMa1 { get; set; } = true;
    public bool UseMa2 { get; set; } = true;
    public bool UseMa3 { get; set; } = true;
    public bool UseMa4 { get; set; } = true;
    public int Ma1Length { get; set; } = 5;
    public int Ma2Length { get; set; } = 20;
    public int Ma3Length { get; set; } = 60;
    public int Ma4Length { get; set; } = 120;

    public FourHmaStrategy(BacktestService backtestService, ExchangeService exchangeService)
        : base(backtestService, exchangeService)
    {
        Title = "4 HMA";
        TargetCoins = "BTC";
        Timeframe = Timeframe.OneDay;
        StartDate = new DateTime(2024, 1, 1);
        TestDays = (DateTime.Now - StartDate).Days + 1;

        ShowDefaultParameters();

        StrategyParameters.Add(new StrategyParameterViewModel(nameof(UseMa1), "Use MA1", UseMa1));
        StrategyParameters.Add(new StrategyParameterViewModel(nameof(UseMa2), "Use MA2", UseMa2));
        StrategyParameters.Add(new StrategyParameterViewModel(nameof(UseMa3), "Use MA3", UseMa3));
        StrategyParameters.Add(new StrategyParameterViewModel(nameof(UseMa4), "Use MA4", UseMa4));
        StrategyParameters.Add(new StrategyParameterViewModel(nameof(Ma1Length), "MA1 Length", Ma1Length));
        StrategyParameters.Add(new StrategyParameterViewModel(nameof(Ma2Length), "MA2 Length", Ma2Length));
        StrategyParameters.Add(new StrategyParameterViewModel(nameof(Ma3Length), "MA3 Length", Ma3Length));
        StrategyParameters.Add(new StrategyParameterViewModel(nameof(Ma4Length), "MA4 Length", Ma4Length));
    }

    protected override void LoadParameters()
    {
        base.LoadParameters();

        foreach (var parameter in StrategyParameters)
        {
            switch (parameter.Value.Name)
            {
                case nameof(UseMa1):
                    UseMa1 = parameter.Value.BoolValue;
                    break;
                case nameof(UseMa2):
                    UseMa2 = parameter.Value.BoolValue;
                    break;
                case nameof(UseMa3):
                    UseMa3 = parameter.Value.BoolValue;
                    break;
                case nameof(UseMa4):
                    UseMa4 = parameter.Value.BoolValue;
                    break;
                case nameof(Ma1Length):
                    Ma1Length = parameter.Value.IntValue;
                    break;
                case nameof(Ma2Length):
                    Ma2Length = parameter.Value.IntValue;
                    break;
                case nameof(Ma3Length):
                    Ma3Length = parameter.Value.IntValue;
                    break;
                case nameof(Ma4Length):
                    Ma4Length = parameter.Value.IntValue;
                    break;
            }
        }
    }

    protected override async Task<bool> LoadOhlcvAsync()
    {
        prepareCandles = 1;
        if (UseMa1) prepareCandles = Math.Max(prepareCandles, Ma1Length + (int)Math.Sqrt(Ma1Length));
        if (UseMa2) prepareCandles = Math.Max(prepareCandles, Ma2Length + (int)Math.Sqrt(Ma2Length));
        if (UseMa3) prepareCandles = Math.Max(prepareCandles, Ma3Length + (int)Math.Sqrt(Ma3Length));
        if (UseMa4) prepareCandles = Math.Max(prepareCandles, Ma4Length + (int)Math.Sqrt(Ma4Length));
        return await base.LoadOhlcvAsync();
    }

    protected override void CreateIndicators()
    {
        foreach (KeyValuePair<string, List<Ohlcv>> ohlcvPair in backtestService.Ohlcvs)
        {
            string name = ohlcvPair.Key;
            backtestService.Indicators[name] = new Dictionary<string, Dictionary<DateTime, double>>()
            {
                { $"MA{Ma1Length}", TechnicalIndicators.HMA(backtestService.ClosePrices[name], Ma1Length) },
                { $"MA{Ma2Length}", TechnicalIndicators.HMA(backtestService.ClosePrices[name], Ma2Length) },
                { $"MA{Ma3Length}", TechnicalIndicators.HMA(backtestService.ClosePrices[name], Ma3Length) },
                { $"MA{Ma4Length}", TechnicalIndicators.HMA(backtestService.ClosePrices[name], Ma4Length) }
            };
        }
    }

    public override async Task<bool> Run()
    {
        if (!await DoInitProcessAsync())
            return false;

        for (int index = prepareCandles; index < btcOhlcvSeries.Count; ++index)
        {
            BacktestStatus status = CreateBacktestStatus(btcOhlcvSeries[index].DateTime);
            double fundPerCoin = backtestService.FreeBalance / TargetCoinNames.Length;

            foreach (string name in TargetCoinNames)
            {
                Ohlcv ohlcv = backtestService.Ohlcvs[name][index];
                Ohlcv prevOhlcv = backtestService.Ohlcvs[name][index - 1];
                Coin coin = backtestService.Coins[name];
                coin.InitOnCandle();
                coin.UpdateBalance(ohlcv.ClosePrice, status);

                // Strategy
                double orderableValuation = fundPerCoin * OrderRatio;
                var ma1Series = backtestService.Indicators[name][$"MA{Ma1Length}"];
                var ma2Series = backtestService.Indicators[name][$"MA{Ma2Length}"];
                var ma3Series = backtestService.Indicators[name][$"MA{Ma3Length}"];
                var ma4Series = backtestService.Indicators[name][$"MA{Ma4Length}"];

                // Close position when exit signal
                if (coin.HasLongPosition)
                {
                    if (TechnicalIndicators.Crossunder(ma1Series, ma2Series, ohlcv.DateTime, prevOhlcv.DateTime))
                    {
                        coin.CloseAllPosition(PositionSide.Long, ohlcv.ClosePrice, MarketFeeRate, status);
                    }
                }
                else if (coin.HasShortPosition)
                {
                    if (TechnicalIndicators.Crossover(ma1Series, ma2Series, ohlcv.DateTime, prevOhlcv.DateTime))
                    {
                        coin.CloseAllPosition(PositionSide.Short, ohlcv.ClosePrice, MarketFeeRate, status);
                    }
                }

                var closePrices = backtestService.ClosePrices[name];
                // Enter position
                if (UseLong && !coin.HasLongPosition)
                {
                    bool entrySignal
                        = TechnicalIndicators.Crossunder(closePrices, ma1Series, ohlcv.DateTime, prevOhlcv.DateTime)
                        && ohlcv.ClosePrice.IsGreaterThan(ma2Series[ohlcv.DateTime])
                        && ohlcv.ClosePrice.IsGreaterThan(ma3Series[ohlcv.DateTime])
                        && ohlcv.ClosePrice.IsGreaterThan(ma4Series[ohlcv.DateTime]);

                    if (entrySignal)
                    {
                        double orderAmount = Math.Round(
                            Leverage * MathUtils.CalculateOrderableAmount(orderableValuation, ohlcv.ClosePrice),
                            coin.AmountPrecision);
                        double orderValuation = orderAmount * ohlcv.ClosePrice;

                        if (orderValuation >= MinimumOrderValuation)
                        {
                            coin.EnterPosition(PositionSide.Long, orderAmount, ohlcv.ClosePrice, LimitFeeRate, status);
                        }
                    }
                }
                if (UseShort && !coin.HasLongPosition && !coin.HasShortPosition)
                {
                    bool entrySignal
                        = TechnicalIndicators.Crossover(closePrices, ma1Series, ohlcv.DateTime, prevOhlcv.DateTime)
                            && ohlcv.ClosePrice.IsLessThan(ma2Series[ohlcv.DateTime])
                            && ohlcv.ClosePrice.IsLessThan(ma3Series[ohlcv.DateTime])
                            && ohlcv.ClosePrice.IsLessThan(ma4Series[ohlcv.DateTime]);

                    if (entrySignal)
                    {
                        double orderAmount = Math.Round(
                            Leverage * MathUtils.CalculateOrderableAmount(orderableValuation, ohlcv.ClosePrice),
                            coin.AmountPrecision);
                        double orderValuation = orderAmount * ohlcv.ClosePrice;

                        if (orderValuation >= MinimumOrderValuation)
                        {
                            coin.EnterPosition(PositionSide.Short, orderAmount, ohlcv.ClosePrice, LimitFeeRate, status);
                        }
                    }
                }
            }

            SummarizeForCandle(status);
        }

        return true;
    }
}
