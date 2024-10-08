using StockBacktester.Crypto;

namespace StockBacktester.Utils;

public static class UserUtil {
  public static TimeSpan ToTimeSpan(this Timeframe timeframe) => timeframe switch {
    Timeframe.OneMinute => TimeSpan.FromMinutes(1),
    Timeframe.ThreeMinutes => TimeSpan.FromMinutes(3),
    Timeframe.FiveMinutes => TimeSpan.FromMinutes(5),
    Timeframe.FifteenMinutes => TimeSpan.FromMinutes(15),
    Timeframe.ThirtyMinutes => TimeSpan.FromMinutes(30),
    Timeframe.OneHour => TimeSpan.FromHours(1),
    Timeframe.TwoHours => TimeSpan.FromHours(2),
    Timeframe.FourHours => TimeSpan.FromHours(4),
    Timeframe.SixHours => TimeSpan.FromHours(6),
    Timeframe.EightHours => TimeSpan.FromHours(8),
    Timeframe.TwelveHours => TimeSpan.FromHours(12),
    Timeframe.OneDay => TimeSpan.FromDays(1),
    Timeframe.ThreeDays => TimeSpan.FromDays(3),
    Timeframe.OneWeek => TimeSpan.FromDays(7),
    Timeframe.OneMonth => TimeSpan.FromDays(30),
    _ => throw new ArgumentException("Invalid timeframe")
  };

  public static string ToStringExpression(this Timeframe timeframe) {
    return timeframe switch {
      Timeframe.OneMinute => "1m",
      Timeframe.ThreeMinutes => "3m",
      Timeframe.FiveMinutes => "5m",
      Timeframe.FifteenMinutes => "15m",
      Timeframe.ThirtyMinutes => "30m",
      Timeframe.OneHour => "1h",
      Timeframe.TwoHours => "2h",
      Timeframe.FourHours => "4h",
      Timeframe.SixHours => "6h",
      Timeframe.EightHours => "8h",
      Timeframe.TwelveHours => "12h",
      Timeframe.OneDay => "1d",
      Timeframe.ThreeDays => "3d",
      Timeframe.OneWeek => "1w",
      Timeframe.OneMonth => "1M",
      _ => timeframe.ToString()
    };
  }

  public static Dictionary<DateTime, double> ClosePriceSeries(
    this Dictionary<DateTime, Ohlcv> ohlcvs) {
    var closePrices = new Dictionary<DateTime, double>(ohlcvs.Count);
    foreach (var kvp in ohlcvs) {
      closePrices[kvp.Key] = kvp.Value.ClosePrice;
    }
    return closePrices;
  }

  public static double NaNToZero(this double value) {
    return double.IsNaN(value) ? 0 : value;
  }
}