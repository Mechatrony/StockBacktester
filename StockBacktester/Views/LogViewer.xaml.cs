using Microsoft.UI.Xaml.Controls;
using StockBacktester.ViewModels;

namespace StockBacktester.Views;

public sealed partial class LogViewer : UserControl
{
    public LogViewerViewModel ViewModel { get; }

    public LogViewer()
    {
        DataContext = new LogViewerViewModel();
        InitializeComponent();
    }
}
