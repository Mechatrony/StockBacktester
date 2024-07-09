using Microsoft.UI.Xaml.Controls;

using StockBacktester.ViewModels;

namespace StockBacktester.Views;

// TODO: Change the grid as appropriate for your app. Adjust the column definitions on DataGridPage.xaml.
// For more details, see the documentation at https://docs.microsoft.com/windows/communitytoolkit/controls/datagrid.
public sealed partial class 데이터표Page : Page
{
    public 데이터표ViewModel ViewModel
    {
        get;
    }

    public 데이터표Page()
    {
        ViewModel = App.GetService<데이터표ViewModel>();
        InitializeComponent();
    }
}
