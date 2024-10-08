using CommunityToolkit.Mvvm.ComponentModel;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockBacktester.ViewModels;

public partial class BacktestResultViewModel : ObservableObject {
  [ObservableProperty]
  private ObservableCollection<PlotModel> _plotModels = [];
}
