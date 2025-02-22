using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backtester.ViewModels;

public partial class IndicatorViewModel(string coinName) : ObservableObject
{
    [ObservableProperty]
    public partial string CoinName { get; set; } = coinName;
    [ObservableProperty]
    public partial string IndicatorName { get; set; } = "";
    [ObservableProperty]
    public partial Dictionary<DateTime, double> Data { get; set; } = new();
    [ObservableProperty]
    public partial bool IsVisible { get; set; } = true;
}
