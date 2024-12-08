using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using StockBacktester.Contracts.Services;
using StockBacktester.Helpers;
using System.Reflection;
using System.Windows.Input;
using Windows.ApplicationModel;

namespace StockBacktester.ViewModels.Pages;

public partial class SettingsPageViewModel : ObservableObject
{
    private readonly IThemeSelectorService themeSelectorService;

    [ObservableProperty]
    private ElementTheme elementTheme;
    [ObservableProperty]
    private string versionDescription;

    public ICommand SwitchThemeCommand { get; }

    public SettingsPageViewModel(IThemeSelectorService themeSelectorService)
    {
        this.themeSelectorService = themeSelectorService;
        elementTheme = this.themeSelectorService.Theme;
        versionDescription = GetVersionDescription();

        SwitchThemeCommand = new RelayCommand<ElementTheme>(
            async (param) =>
            {
                if (ElementTheme != param)
                {
                    ElementTheme = param;
                    await this.themeSelectorService.SetThemeAsync(param);
                }
            });
    }

    private static string GetVersionDescription()
    {
        Version version;

        if (RuntimeHelper.IsMSIX)
        {
            PackageVersion packageVersion = Package.Current.Id.Version;
            version = new Version(
                packageVersion.Major,
                packageVersion.Minor,
                packageVersion.Build,
                packageVersion.Revision);
        }
        else
        {
            version = Assembly.GetExecutingAssembly().GetName().Version!;
        }

        return $"{"AppDisplayName".GetLocalized()} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }
}
