using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StockBacktester.Models;

namespace StockBacktester.ViewModels;

public partial class MainViewModel : ObservableRecipient {
  public MainViewModel() {
  }

  [RelayCommand]
  private void Test() {
    Logger.Log("Hello");
  }
}
