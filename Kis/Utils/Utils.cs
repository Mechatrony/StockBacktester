using Kis.Enums;

namespace Kis.Utils;

public static class Utils {
  public static string ToDescription(this Timeframe timeframe) {
    return timeframe switch {
      Timeframe.OneDay => "D",
      Timeframe.OneWeek => "W",
      Timeframe.OneMonth => "M",
      Timeframe.OneYear => "Y",
      _ => timeframe.ToString(),
    };
  }
}
