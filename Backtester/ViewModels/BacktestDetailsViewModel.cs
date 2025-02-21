using CommunityToolkit.Mvvm.ComponentModel;
using Backtester.Crypto;
using Backtester.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backtester.ViewModels;

public partial class BacktestDetailsViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<BacktestStatus> backtestDetails = new();

    public BacktestDetailsViewModel(BacktestService backtest)
    {
        BacktestDetails = new ObservableCollection<BacktestStatus>(backtest.BacktestDetails);
    }
}
