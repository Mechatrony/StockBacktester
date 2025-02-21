using Microsoft.UI.Xaml.Controls;
using Backtester.ViewModels.UserControls;

namespace Backtester.Views.UserControls;

public sealed partial class LogViewer : UserControl
{
    public LogViewerViewModel ViewModel { get; }

    public LogViewer()
    {
        DataContext = new LogViewerViewModel();
        InitializeComponent();
    }
}
