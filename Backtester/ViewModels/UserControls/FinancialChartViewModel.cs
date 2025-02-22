using CommunityToolkit.Mvvm.ComponentModel;
using Backtester.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backtester.ViewModels.UserControls;

public partial class FinancialChartViewModel(string title, List<Ohlcv> ohlcvs) : ObservableObject
{
    [ObservableProperty]
    public partial string Title { get; set; } = title;
    [ObservableProperty]
    public partial List<Ohlcv> Ohlcvs { get; set; } = ohlcvs;
    [ObservableProperty]
    public partial List<IndicatorViewModel> Indicators { get; set; } = new();

    public IndicatorViewModel? Indicator1 => Indicators.ElementAtOrDefault(0);
    public IndicatorViewModel? Indicator2 => Indicators.ElementAtOrDefault(1);
    public IndicatorViewModel? Indicator3 => Indicators.ElementAtOrDefault(2);
    public IndicatorViewModel? Indicator4 => Indicators.ElementAtOrDefault(3);
}