using CommunityToolkit.Mvvm.ComponentModel;
using OxyPlot;
using Backtester.Crypto;
using System.Collections.ObjectModel;

namespace Backtester.ViewModels.UserControls;

public partial class BacktestResultViewModel : ObservableObject
{
    // TODO: Migrate Syncfusion to OxyPlot
    [ObservableProperty]
    private ObservableCollection<PlotModel> plotModels = [];

    [ObservableProperty]
    private ObservableCollection<FinancialChartViewModel> charts = new();
    [ObservableProperty]
    private ObservableCollection<BacktestStatus> backtestDetails = new();
    [ObservableProperty]
    private Dictionary<DateTime, double> btcRorSeries = new();
    [ObservableProperty]
    private double btcRor = 0;
    [ObservableProperty]
    private double ror = 0;
    [ObservableProperty]
    private double cagr = 0;
    [ObservableProperty]
    private double mdd = 0;
    [ObservableProperty]
    private double winRate = 0;
    [ObservableProperty]
    private int winCount = 0;
    [ObservableProperty]
    private int tradeCount = 0;
    [ObservableProperty]
    private double pnlRatio = 0;
    [ObservableProperty]
    private double sharpe = 0;
    [ObservableProperty]
    private double sortino = 0;
    [ObservableProperty]
    private double kelly = 0;
}
