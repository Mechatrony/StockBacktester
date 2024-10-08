using System.ComponentModel;

namespace StockBacktester.Crypto;

public enum CryptoExchange {
  None = -1,
  BinanceFutures,
  BinanceSpot,
  Upbit,
}

public enum PositionSide {
  Long,
  Short,
}

public enum MarketType {
  Spot,
  Futures,
}

public enum Timeframe {
  [Description("1m")]OneMinute,
  [Description("3m")]ThreeMinutes,
  [Description("5m")]FiveMinutes,
  [Description("15m")]FifteenMinutes,
  [Description("30m")]ThirtyMinutes,
  [Description("1h")]OneHour,
  [Description("2h")]TwoHours,
  [Description("4h")]FourHours,
  [Description("6h")]SixHours,
  [Description("8h")]EightHours,
  [Description("12h")]TwelveHours,
  [Description("1d")]OneDay,
  [Description("3d")]ThreeDays,
  [Description("W")]OneWeek,
  [Description("M")]OneMonth,
  [Description("Y")]OneYear,
}