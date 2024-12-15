using Microsoft.UI.Xaml.Controls;
using StockBacktester.ViewModels.Pages;

namespace StockBacktester.Views;

// TODO: Set the URL for your privacy policy by updating SettingsPage_PrivacyTermsLink.NavigateUri in Resources.resw.
public sealed partial class SettingsPage : Page
{
    public SettingsPage()
    {
        DataContext = App.GetService<SettingsPageViewModel>();
        InitializeComponent();
    }
}
