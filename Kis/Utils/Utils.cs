using Kis.Enums;

namespace Kis.Utils;

public static class Utils {
  public static string ToDescription(this Timeframe timeframe) {
    return timeframe switch {
      Timeframe.OneDay => "D",
      Timeframe.OneWeek => "W",
      Timeframe.OneMonth => "M",
      Timeframe.OneYear => "Y",
      _ => timeframe.ToString()
    };
  }

  public static TimeSpan ToTimeSpan(this Timeframe timeframe) {
    return timeframe switch {
      Timeframe.OneDay => TimeSpan.FromDays(1),
      Timeframe.OneWeek => TimeSpan.FromDays(7),
      Timeframe.OneMonth => TimeSpan.FromDays(30),
      Timeframe.OneYear => TimeSpan.FromDays(365),
      _ => TimeSpan.Zero
    };
  }
}
