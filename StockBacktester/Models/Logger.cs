using Meziantou.Framework.WPF.Collections;

namespace StockBacktester.Models;

public enum LogLevel {
  Info,
  Warn,
  Error,
}

public class Logger {
  // Singleton
  public static Logger Instance { get; } = new Logger();

  private Logger() { }

  public static ConcurrentObservableCollection<LogEntry> LogEntries { get; } = new();
  public static int Capacity { get; set; } = 1000;

  public static void Log(string text, LogLevel logLevel = LogLevel.Info) {
    LogEntries.Add(new LogEntry(DateTime.Now, text, logLevel));

    while (LogEntries.Count > Capacity) {
      LogEntries.RemoveAt(0);
    }
  }
}
