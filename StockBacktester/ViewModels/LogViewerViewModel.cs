using CommunityToolkit.Mvvm.ComponentModel;
using Meziantou.Framework.WPF.Collections;
using StockBacktester.Models;

namespace StockBacktester.ViewModels;

public class LogViewerViewModel : ObservableObject {
  public static IReadOnlyObservableCollection<LogEntry> LogEntries => Logger.LogEntries.AsObservable;

  public LogViewerViewModel() {
    // TODO: 임시 코드임. Logger 테스트 후 삭제할 것.

    // Multi thread
    Task.Run(() => {
      for (int index = 0; index < 10000; ++index) {
        Logger.Log($"Multi thread {index}", LogLevel.Warn);
      }
    });

    // UI thread
    for (int index = 0; index < 10000; ++index) {
      Logger.Log($"UI thread {index}");
    }
  }
}
