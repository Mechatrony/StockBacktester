using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Backtester.Contracts.Services;
using Backtester.Helpers;
using System.Reflection;
using Windows.ApplicationModel;

namespace Backtester.ViewModels.Pages;

public partial class SettingsPageViewModel(IThemeSelectorService themeSelectorService) : ObservableObject
{
    [ObservableProperty]
    public partial ElementTheme ElementTheme { get; set; } = themeSelectorService.Theme;
    [ObservableProperty]
    public partial string VersionDescription { get; set; } = GetVersionDescription();

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

    [RelayCommand]
    private async Task SwitchTheme(ElementTheme theme)
    {
        if (ElementTheme != theme)
        {
            ElementTheme = theme;
            await themeSelectorService.SetThemeAsync(theme);
        }
    }
}
