using Microsoft.UI.Xaml.Controls;

using StockBacktester.ViewModels;

namespace StockBacktester.Views;

public sealed partial class 콘텐츠표Page : Page
{
    public 콘텐츠표ViewModel ViewModel
    {
        get;
    }

    public 콘텐츠표Page()
    {
        ViewModel = App.GetService<콘텐츠표ViewModel>();
        InitializeComponent();
    }
}
