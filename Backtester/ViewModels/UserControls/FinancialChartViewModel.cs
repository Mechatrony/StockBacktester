using CommunityToolkit.Mvvm.ComponentModel;
using Backtester.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backtester.ViewModels.UserControls;

public partial class FinancialChartViewModel : ObservableObject
{
    [ObservableProperty]
    private string title = string.Empty;
    [ObservableProperty]
    private List<Ohlcv> ohlcvs;
    [ObservableProperty]
    private List<IndicatorViewModel> indicators = new();

    public IndicatorViewModel? Indicator1 => Indicators.ElementAtOrDefault(0);
    public IndicatorViewModel? Indicator2 => Indicators.ElementAtOrDefault(1);
    public IndicatorViewModel? Indicator3 => Indicators.ElementAtOrDefault(2);
    public IndicatorViewModel? Indicator4 => Indicators.ElementAtOrDefault(3);

    public FinancialChartViewModel(string title, List<Ohlcv> ohlcvs)
    {
        Title = title;
        Ohlcvs = ohlcvs;
    }
}