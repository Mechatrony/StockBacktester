namespace Backtester.Crypto;

public class Ohlcv(DateTime dateTime, double open, double high, double low, double close, double volume)
{
    public DateTime DateTime { get; set; } = dateTime;
    public double OpenPrice { get; set; } = open;
    public double HighPrice { get; set; } = high;
    public double LowPrice { get; set; } = low;
    public double ClosePrice { get; set; } = close;
    public double Volume { get; set; } = volume;
    public bool IsFirst { get; set; } = false;
}