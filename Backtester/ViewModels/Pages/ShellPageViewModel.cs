using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Backtester.Contracts.Services;
using Backtester.Views.Pages;

namespace Backtester.ViewModels.Pages;

public partial class ShellPageViewModel : ObservableRecipient
{
    [ObservableProperty]
    private bool isBackEnabled;
    [ObservableProperty]
    private object? selectedNavigationViewItem;

    public INavigationService NavigationService { get; }
    public INavigationViewService NavigationViewService { get; }

    public ShellPageViewModel(INavigationService navigationService, INavigationViewService navigationViewService)
    {
        NavigationService = navigationService;
        NavigationService.Navigated += OnNavigated;
        NavigationViewService = navigationViewService;
    }

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        IsBackEnabled = NavigationService.CanGoBack;

        if (e.SourcePageType == typeof(SettingsPage))
        {
            SelectedNavigationViewItem = NavigationViewService.SettingsItem;
            return;
        }

        NavigationViewItem? selectedItem = NavigationViewService.GetSelectedItem(e.SourcePageType);
        if (selectedItem != null)
        {
            SelectedNavigationViewItem = selectedItem;
        }
    }
}
