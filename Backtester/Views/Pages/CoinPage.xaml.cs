using Microsoft.UI.Xaml.Controls;
using Backtester.ViewModels.Pages;

namespace Backtester.Views.Pages;

public sealed partial class CoinPage : Page
{
    public CoinPage()
    {
        DataContext = App.GetService<CoinPageViewModel>();
        InitializeComponent();
    }
}
