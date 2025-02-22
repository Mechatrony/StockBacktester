using CommunityToolkit.Mvvm.ComponentModel;
using OxyPlot;
using Backtester.Crypto;
using System.Collections.ObjectModel;

namespace Backtester.ViewModels.UserControls;

public partial class BacktestResultViewModel : ObservableObject
{
    // TODO: Migrate Syncfusion to OxyPlot
    [ObservableProperty]
    public partial ObservableCollection<PlotModel> PlotModels { get; set; } = [];

    [ObservableProperty]
    public partial ObservableCollection<FinancialChartViewModel> Charts { get; set; } = new();
    [ObservableProperty]
    public partial ObservableCollection<BacktestStatus> BacktestDetails { get; set; } = new();
    [ObservableProperty]
    public partial Dictionary<DateTime, double> BtcRorSeries { get; set; } = new();
    [ObservableProperty]
    public partial double BtcRor { get; set; } = 0;
    [ObservableProperty]
    public partial double Ror { get; set; } = 0;
    [ObservableProperty]
    public partial double Cagr { get; set; } = 0;
    [ObservableProperty]
    public partial double Mdd { get; set; } = 0;
    [ObservableProperty]
    public partial double WinRate { get; set; } = 0;
    [ObservableProperty]
    public partial int WinCount { get; set; } = 0;
    [ObservableProperty]
    public partial int TradeCount { get; set; } = 0;
    [ObservableProperty]
    public partial double PnlRatio { get; set; } = 0;
    [ObservableProperty]
    public partial double Sharpe { get; set; } = 0;
    [ObservableProperty]
    public partial double Sortino { get; set; } = 0;
    [ObservableProperty]
    public partial double Kelly { get; set; } = 0;
}
