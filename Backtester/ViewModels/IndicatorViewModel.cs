using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backtester.ViewModels;

public partial class IndicatorViewModel : ObservableObject
{
    [ObservableProperty]
    private Dictionary<DateTime, double> data = new Dictionary<DateTime, double>();
    [ObservableProperty]
    private string coinName = string.Empty;
    [ObservableProperty]
    private string indicatorName = string.Empty;
    [ObservableProperty]
    private bool isVisible = true;

    public IndicatorViewModel(string coinName, string indicatorName, Dictionary<DateTime, double> data)
    {
        CoinName = coinName;
        IndicatorName = indicatorName;
        Data = data;
    }

    public IndicatorViewModel()
    {

    }
}
