namespace StockBacktester.Models;

public class LogEntry(DateTime dateTime, string message, LogLevel logLevel)
{
    public DateTime DateTime { get; set; } = dateTime;
    public string Message { get; set; } = message;
    public LogLevel LogLevel { get; set; } = logLevel;
}
