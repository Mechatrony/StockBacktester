using Microsoft.UI.Xaml.Controls;
using StockBacktester.ViewModels.Pages;

namespace StockBacktester.Views;

public sealed partial class CoinPage : Page
{
    public CoinPage()
    {
        DataContext = App.GetService<CoinPageViewModel>();
        InitializeComponent();
    }
}
