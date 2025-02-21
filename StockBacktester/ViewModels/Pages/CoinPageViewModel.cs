using CommunityToolkit.Mvvm.ComponentModel;
using Backtester.Crypto.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backtester.ViewModels.Pages;

public partial class CoinPageViewModel : ObservableObject
{
    [ObservableProperty]
    private string title = "Coin Page";

    [ObservableProperty]
    private StrategyBase? selectedStrategy;
}
