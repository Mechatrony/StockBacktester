using Microsoft.UI.Xaml.Controls;
using StockBacktester.ViewModels;

namespace StockBacktester.Views;

public sealed partial class LogViewer : UserControl {
  public LogViewerViewModel ViewModel { get; }

  public LogViewer() {
    //ViewModel = new LogViewerViewModel();
    DataContext = new LogViewerViewModel();
    this.InitializeComponent();
  }
}
