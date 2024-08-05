using CommunityToolkit.Mvvm.ComponentModel;
using Meziantou.Framework.WPF.Collections;
using StockBacktester.Models;

namespace StockBacktester.ViewModels;

public class LogViewerViewModel : ObservableObject {
  public static IReadOnlyObservableCollection<LogEntry> LogEntries => Logger.LogEntries.AsObservable;

  public LogViewerViewModel() {
  }
}
