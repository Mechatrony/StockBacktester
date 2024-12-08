using CommunityToolkit.Mvvm.ComponentModel;
using StockBacktester.Crypto.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockBacktester.ViewModels.Pages;

public partial class CoinPageViewModel : ObservableObject {
  [ObservableProperty]
  private StrategyBase selectedStrategy;
}
