using System;

namespace Kis;

public class Ohlcv(DateTime dateTime, double openPrice,
  double highPrice, double lowPrice, double closePrice,
  UInt64 stockVolume, UInt64 currencyVolume) {

  public DateTime DateTime { get; set; } = dateTime;
  public double OpenPrice { get; set; } = openPrice;
  public double HighPrice { get; set; } = highPrice;
  public double LowPrice { get; set; } = lowPrice;
  public double ClosePrice { get; set; } = closePrice;
  public UInt64 StockVolume { get; set; } = stockVolume;
  public UInt64 CurrencyVolume { get; set; } = currencyVolume;

  public override string ToString() {
    return $"DateTime: {DateTime:yyyy-MM-dd}, OpenPrice: {OpenPrice}, HighPrice: {HighPrice}" +
      $", LowPrice: {LowPrice}, ClosePrice: {ClosePrice}" +
      $", StockVolume: {StockVolume}, CurrencyVolume: {CurrencyVolume}";
  }
}
