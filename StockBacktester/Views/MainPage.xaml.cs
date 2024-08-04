using Microsoft.UI.Xaml.Controls;

using StockBacktester.ViewModels;

namespace StockBacktester.Views;

public sealed partial class MainPage : Page {
  public MainPage() {
    DataContext = App.GetService<MainViewModel>();
    InitializeComponent();
  }
}
