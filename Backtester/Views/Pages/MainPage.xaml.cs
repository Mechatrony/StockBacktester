using Microsoft.UI.Xaml.Controls;
using Backtester.ViewModels.Pages;

namespace Backtester.Views.Pages;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        DataContext = App.GetService<MainPageViewModel>();
        InitializeComponent();
    }
}
