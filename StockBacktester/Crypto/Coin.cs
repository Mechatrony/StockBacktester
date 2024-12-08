using StockBacktester.Crypto.Strategies;
using StockBacktester.Services;
using StockBacktester.Utils;
using StockBacktester.ViewModels.Pages;
using System.Collections.Specialized;

namespace StockBacktester.Crypto;


public class Coin(CoinInfo coinInfo, int leverage)
{
    private static readonly StrategyBase strategy = App.GetService<CoinPageViewModel>().SelectedStrategy;
    private static readonly BacktestService backtestService = new();

    public static double MarketFeeRate { get; set; } = 0.0005d;
    public static double LimitFeeRate { get; set; } = 0.0002d;

    public string Name { get; set; } = coinInfo.Name;
    public string Symbol { get; } = coinInfo.Name + "USDT";
    public string Ticker { get; } = coinInfo.Name + "/USDT";
    public DateTime ListingDate { get; } = coinInfo.ListingDate;
    public double CirculatingSupply { get; } = coinInfo.CirculatingSupply;

    public OrderedDictionary OpenLongOrders { get; } = new();
    public OrderedDictionary OpenShortOrders { get; } = new();
    public OrderedDictionary CloseLongOrders { get; } = new();
    public OrderedDictionary CloseShortOrders { get; } = new();

    public double Price { get; }
    public int Leverage { get; set; } = leverage;
    public int AmountPrecision { get; set; } = coinInfo.AmountPrecision;
    public int PricePrecision { get; set; } = coinInfo.PricePrecision;

    public bool Error { get; }

    public bool IsTraded { get; set; }
    public bool IsPositionOpened { get; set; }
    public bool IsPositionClosed { get; set; }
    public bool IsLongOpened { get; set; }
    public bool IsLongClosed { get; set; }
    public bool IsShortOpened { get; set; }
    public bool IsShortClosed { get; set; }
    public bool IsLongLiquidated { get; set; }
    public bool IsShortLiquidated { get; set; }
    public bool HasLongPosition => LongAmount.IsGreaterThan(0);
    public bool HasShortPosition => ShortAmount.IsGreaterThan(0);

    /// <summary>
    /// Leverage applied amount
    /// </summary>
    public double LongAmount { get; set; }
    public double ShortAmount { get; set; }
    /// <summary>
    /// Leverage applied Valuation
    /// </summary>
    public double LongValuation { get; set; }
    public double ShortValuation { get; set; }

    public double LongEntryPrice { get; set; }
    public double LongMargin { get; set; }
    public double LongLiquidationPrice { get; set; } = double.NegativeInfinity;
    public double LongUnrealizedPnl { get; set; }
    public double LongRealizedPnl { get; set; }
    public double LongYield { get; set; }
    public double LongYieldWithLeverage { get; set; }
    public double MaxLongYieldWithLeverage { get; set; }
    public double MinLongYieldWithLeverage { get; set; }
    public int LongHoldingPeriod { get; set; }
    public bool LongStopLossExist { get; set; }
    public bool LongTakeProfitExist { get; set; }
    public double LongStopLossPrice { get; set; }
    public double LongTakeProfitPrice { get; set; }

    public double ShortEntryPrice { get; set; }
    public double ShortMargin { get; set; }
    public double ShortLiquidationPrice { get; set; } = double.PositiveInfinity;
    public double ShortUnrealizedPnl { get; set; }
    public double ShortRealizedPnl { get; set; }
    public double ShortYield { get; set; }
    public double ShortYieldWithLeverage { get; set; }
    public double MaxShortYieldWithLeverage { get; set; }
    public double MinShortYieldWithLeverage { get; set; }
    public int ShortHoldingPeriod { get; set; }
    public bool ShortStopLossExist { get; set; }
    public bool ShortTakeProfitExist { get; set; }
    public double ShortStopLossPrice { get; set; }
    public double ShortTakeProfitPrice { get; set; }

    public int WinCount { get; set; }
    public int WinLongCount { get; set; }
    public int WinShortCount { get; set; }
    public int LoseCount { get; set; }
    public int LoseLongCount { get; set; }
    public int LoseShortCount { get; set; }

    private void ResetLongPosition()
    {
        CancelStopLoss(PositionSide.Long);
        LongAmount = 0;
        LongValuation = 0;
        LongEntryPrice = 0;
        LongMargin = 0;
        LongLiquidationPrice = double.NegativeInfinity;
        LongUnrealizedPnl = 0;
        LongRealizedPnl = 0;
        LongYield = 0;
        LongYieldWithLeverage = 0;
        LongHoldingPeriod = 0;
        LongTakeProfitExist = false;
        LongTakeProfitPrice = 0;
    }

    private void ResetShortPosition()
    {
        CancelStopLoss(PositionSide.Short);
        ShortAmount = 0;
        ShortValuation = 0;
        ShortEntryPrice = 0;
        ShortMargin = 0;
        ShortLiquidationPrice = double.PositiveInfinity;
        ShortUnrealizedPnl = 0;
        ShortRealizedPnl = 0;
        ShortYield = 0;
        ShortYieldWithLeverage = 0;
        ShortHoldingPeriod = 0;
        ShortTakeProfitExist = false;
        ShortTakeProfitPrice = 0;
    }

    public void InitOnCandle()
    {
        IsTraded = false;
        IsPositionOpened = false;
        IsPositionClosed = false;
        IsLongOpened = false;
        IsLongClosed = false;
        IsShortOpened = false;
        IsShortClosed = false;
        IsLongLiquidated = false;
        IsShortLiquidated = false;

        if (LongAmount.IsZero())
        {
            ResetLongPosition();
        }
        else
        {
            ++LongHoldingPeriod;
        }

        if (ShortAmount.IsZero())
        {
            ResetShortPosition();
        }
        else
        {
            ++ShortHoldingPeriod;
        }
    }

    private static double CalculateYield(double entryPrice, double exitPrice)
    {
        return (exitPrice - entryPrice) / entryPrice * 100d;
    }

    private bool TryLiquidate(PositionSide positionSide, BacktestStatus status)
    {
        if (positionSide == PositionSide.Long)
        {
            backtestService.Orders[Name].Add(new OrderInfo
            {
                CoinName = Name,
                PositionSide = positionSide,
                DateTime = status.DateTime,
                IsEntry = false,
                Price = LongLiquidationPrice,
            });
            backtestService.TradeResults[Name].Add(new CompletedTradeResult
            {
                CoinName = Name,
                PositionSide = positionSide,
                DateTime = status.DateTime,
                EntryPrice = LongEntryPrice,
                ClosePrice = LongLiquidationPrice,
                Pnl = LongAmount * LongEntryPrice - LongMargin,
                Yield = -100d,
            });
            ResetLongPosition();
            return true;
        }
        else if (positionSide == PositionSide.Short)
        {
            backtestService.Orders[Name].Add(new OrderInfo
            {
                CoinName = Name,
                PositionSide = positionSide,
                DateTime = status.DateTime,
                IsEntry = false,
                Price = ShortLiquidationPrice,
            });
            backtestService.TradeResults[Name].Add(new CompletedTradeResult
            {
                CoinName = Name,
                PositionSide = positionSide,
                DateTime = status.DateTime,
                EntryPrice = ShortEntryPrice,
                ClosePrice = ShortLiquidationPrice,
                Pnl = ShortAmount * ShortEntryPrice - ShortMargin,
                Yield = -100d,
            });
            ResetShortPosition();
            return true;
        }
        return false;
    }

    // TODO: Test stop loss logic
    public void UpdateBalance(double price, BacktestStatus status)
    {
        price = Math.Round(price, PricePrecision);

        if (!LongAmount.IsZero())
        {
            if (TryFillStopLoss(PositionSide.Long, price, status))
            {
            }
            else if (price.IsSameOrLessThan(LongLiquidationPrice))
            {
                //ResetLongPosition();
                if (TryLiquidate(PositionSide.Long, status))
                {
                }
            }
            else
            {
                LongValuation = LongAmount * price;
                LongUnrealizedPnl = LongAmount * (price - LongEntryPrice);
                LongYield = CalculateYield(LongEntryPrice, price);
                LongYieldWithLeverage = LongYield * Leverage;
            }
        }

        if (!ShortAmount.IsZero())
        {
            if (TryFillStopLoss(PositionSide.Short, price, status))
            {
            }
            else if (price.IsSameOrGreaterThan(ShortLiquidationPrice))
            {
                //ResetShortPosition();
                if (TryLiquidate(PositionSide.Short, status))
                {
                }
            }
            else
            {
                ShortValuation = ShortAmount * (2 * ShortEntryPrice - price);
                ShortUnrealizedPnl = ShortAmount * (ShortEntryPrice - price);
                ShortYield = CalculateYield(ShortEntryPrice, price) * -1;
                ShortYieldWithLeverage = ShortYield * Leverage;
            }
        }
    }

    /// <summary>
    /// Enter long position
    /// </summary>
    /// <param name="positionSide">Long or Short</param>
    /// <param name="amount">Leverage applied amount</param>
    /// <param name="price"></param>
    public void EnterPosition(PositionSide positionSide,
      double amount, double price, double feeRate, BacktestStatus backtestStatus)
    {
        amount = Math.Round(amount, AmountPrecision);
        price = Math.Round(price, PricePrecision);

        double additionValuation = amount * price;
        double additionalMargin = additionValuation * (Leverage - 1) / Leverage;
        double fee = additionValuation * feeRate;
        double cost = additionValuation - additionalMargin + fee;

        if (backtestStatus.FreeBalance < cost) return;
        backtestStatus.FreeBalance -= cost;

        if (positionSide == PositionSide.Long)
        {
            LongEntryPrice += (price - LongEntryPrice) * amount / (amount + LongAmount);
            LongAmount += amount;
            LongMargin += additionalMargin;
            LongValuation += additionValuation;

            LongLiquidationPrice = LongEntryPrice * (Leverage - 1) / Leverage;
        }
        else if (positionSide == PositionSide.Short)
        {
            ShortEntryPrice += (price - ShortEntryPrice) * amount / (amount + ShortAmount);
            ShortAmount += amount;
            ShortMargin += additionalMargin;
            ShortValuation += additionValuation;

            ShortLiquidationPrice = ShortEntryPrice * (1 + 1 / Leverage);
        }

        if (!backtestStatus.TradedCoins.Contains(this))
        {
            backtestStatus.TradedCoins.Add(this);
        }

        backtestService.Orders[Name].Add(new OrderInfo
        {
            CoinName = Name,
            DateTime = backtestStatus.DateTime,
            PositionSide = positionSide,
            IsEntry = true,
            Price = price,
        });
    }

    public bool CloseAllPosition(PositionSide positionSide, double price,
      double feeRate, BacktestStatus backtestStatus)
    {
        double amount = positionSide == PositionSide.Long ? LongAmount : ShortAmount;
        return ClosePosition(positionSide, amount, price, feeRate, backtestStatus);
    }

    public bool ClosePosition(PositionSide positionSide, double amount, double price,
      double feeRate, BacktestStatus backtestStatus)
    {
        amount = Math.Round(amount, AmountPrecision);
        price = Math.Round(price, PricePrecision);

        if (positionSide == PositionSide.Long)
        {
            if (LongAmount.IsZero()) return false;

            amount = Math.Min(amount, LongAmount);
            double deductionValuation = amount * price;
            double entryValuation = amount * LongEntryPrice;
            double deductionMargin = entryValuation * (Leverage - 1) / Leverage;
            double fee = deductionValuation * feeRate;

            if (!price.IsCloseTo(LongEntryPrice))
            {
                if (price > LongEntryPrice)
                {
                    ++WinCount;
                    ++WinLongCount;
                    ++backtestStatus.WinCount;
                }
                else
                {
                    ++LoseCount;
                    ++LoseLongCount;
                    ++backtestStatus.LoseCount;
                }
            }

            backtestService.TradeResults[Name].Add(new CompletedTradeResult
            {
                CoinName = Name,
                DateTime = backtestStatus.DateTime,
                PositionSide = positionSide,
                EntryPrice = LongEntryPrice,
                ClosePrice = price,
                Pnl = amount * (price - LongEntryPrice) - deductionMargin - fee,
                Yield = CalculateYield(LongEntryPrice, price)
            });

            backtestStatus.FreeBalance += deductionValuation - deductionMargin - fee;
            LongAmount -= amount;
            LongMargin -= deductionMargin;
            LongValuation -= deductionValuation;
            LongEntryPrice = LongAmount.IsZero() ? 0d : LongValuation / LongAmount;

            if (!backtestStatus.TradedCoins.Contains(this))
            {
                backtestStatus.TradedCoins.Add(this);
            }
            if (LongAmount.IsZero())
            {
                ResetLongPosition();
            }
        }
        else if (positionSide == PositionSide.Short)
        {
            if (ShortAmount.IsZero()) return false;

            amount = Math.Min(amount, ShortAmount);
            double deductionValuation = amount * (2 * ShortEntryPrice - price);
            double entryValuation = amount * ShortEntryPrice;
            double deductionMargin = entryValuation * (Leverage - 1) / Leverage;
            double fee = deductionValuation * feeRate;

            if (!price.IsCloseTo(ShortEntryPrice))
            {
                if (price < ShortEntryPrice)
                {
                    ++WinCount;
                    ++WinShortCount;
                    ++backtestStatus.WinCount;
                }
                else
                {
                    ++LoseCount;
                    ++LoseShortCount;
                    ++backtestStatus.LoseCount;
                }
            }

            backtestService.TradeResults[Name].Add(new CompletedTradeResult
            {
                CoinName = Name,
                DateTime = backtestStatus.DateTime,
                PositionSide = positionSide,
                EntryPrice = ShortEntryPrice,
                ClosePrice = price,
                Pnl = amount * (price - ShortEntryPrice) - deductionMargin - fee,
                Yield = CalculateYield(ShortEntryPrice, price)
            });

            backtestStatus.FreeBalance += deductionValuation - deductionMargin - fee;
            ShortAmount -= amount;
            ShortMargin -= deductionMargin;
            ShortValuation -= deductionValuation;
            ShortEntryPrice = ShortAmount.IsZero() ? 0d : ShortValuation / ShortAmount;

            if (!backtestStatus.TradedCoins.Contains(this))
            {
                backtestStatus.TradedCoins.Add(this);
            }
            if (ShortAmount.IsZero())
            {
                ResetShortPosition();
            }
        }

        backtestService.Orders[Name].Add(new OrderInfo
        {
            CoinName = Name,
            DateTime = backtestStatus.DateTime,
            PositionSide = positionSide,
            IsEntry = false,
            Price = price,
        });

        return true;
    }

    public void SetStopLoss(PositionSide positionSide, double triggerPrice)
    {
        if (positionSide == PositionSide.Long)
        {
            if (LongAmount.IsZero()) return;
            if (triggerPrice < LongLiquidationPrice) return;

            LongStopLossPrice = Math.Round(triggerPrice, PricePrecision);
            LongStopLossExist = true;
        }
        else if (positionSide == PositionSide.Short)
        {
            if (ShortAmount.IsZero()) return;
            if (triggerPrice > ShortLiquidationPrice) return;

            ShortStopLossPrice = Math.Round(triggerPrice, PricePrecision);
            ShortStopLossExist = true;
        }
    }

    private bool TryFillStopLoss(PositionSide positionSide, double price, BacktestStatus status)
    {
        if (positionSide == PositionSide.Long)
        {
            if (LongAmount.IsZero()) return false;
            if (!LongStopLossExist) return false;
            if (!price.IsSameOrLessThan(LongStopLossPrice)) return false;

            if (CloseAllPosition(PositionSide.Long, LongStopLossPrice, strategy.MarketFeeRate, status))
            {
                LongStopLossExist = false;
                return true;
            }
        }
        else if (positionSide == PositionSide.Short)
        {
            if (ShortAmount.IsZero()) return false;
            if (!ShortStopLossExist) return false;
            if (!price.IsSameOrGreaterThan(ShortStopLossPrice)) return false;

            if (CloseAllPosition(PositionSide.Short, ShortStopLossPrice, strategy.MarketFeeRate, status))
            {
                ShortStopLossExist = false;
                return true;
            }
        }

        return false;
    }

    public void CancelStopLoss(PositionSide positionSide)
    {
        if (positionSide == PositionSide.Long)
        {
            LongStopLossExist = false;
            LongStopLossPrice = double.NaN;
        }
        else if (positionSide == PositionSide.Short)
        {
            ShortStopLossExist = false;
            ShortStopLossPrice = double.NaN;
        }
    }
}
