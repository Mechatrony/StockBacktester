using Microsoft.UI.Xaml.Controls;
using StockBacktester.ViewModels.Pages;

namespace StockBacktester.Views;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        DataContext = App.GetService<MainPageViewModel>();
        InitializeComponent();
    }
}
