using Microsoft.UI.Xaml.Controls;
using Backtester.ViewModels.Pages;

namespace Backtester.Views;

public sealed partial class CoinPage : Page
{
    public CoinPage()
    {
        DataContext = App.GetService<CoinPageViewModel>();
        InitializeComponent();
    }
}
