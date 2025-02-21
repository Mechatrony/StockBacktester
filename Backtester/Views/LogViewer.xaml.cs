using Microsoft.UI.Xaml.Controls;
using Backtester.ViewModels;

namespace Backtester.Views;

public sealed partial class LogViewer : UserControl
{
    public LogViewerViewModel ViewModel { get; }

    public LogViewer()
    {
        DataContext = new LogViewerViewModel();
        InitializeComponent();
    }
}
