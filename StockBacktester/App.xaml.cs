using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Backtester.Activation;
using Backtester.Contracts.Services;
using Backtester.Crypto.Exchange;
using Backtester.Models;
using Backtester.Services;
using Backtester.ViewModels;
using Backtester.ViewModels.Pages;
using Backtester.Views;

namespace Backtester;

public partial class App : Application
{
    public IHost Host { get; }

    public static T GetService<T>() where T : class
    {
        if ((App.Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException(
                $"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }

        return service;
    }

    public static WindowEx MainWindow { get; } = new MainWindow();

    public static UIElement? AppTitlebar { get; set; }

    public App()
    {
        InitializeComponent();

        Host = Microsoft.Extensions.Hosting.Host
            .CreateDefaultBuilder()
            .UseContentRoot(AppContext.BaseDirectory)
            .ConfigureServices((context, services) =>
                {
                    // Default Activation Handler
                    services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

                    // Other Activation Handlers

                    // Services
                    services.AddSingleton<ILocalSettingsService, LocalSettingsService>();
                    services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
                    services.AddTransient<INavigationViewService, NavigationViewService>();
                    services.AddSingleton<IActivationService, ActivationService>();
                    services.AddSingleton<IPageService, PageService>();
                    services.AddSingleton<INavigationService, NavigationService>();

                    services.AddSingleton<FileService>();
                    services.AddSingleton<ExchangeService>();
                    services.AddSingleton<BacktestService>();

                    // Views and ViewModels
                    services.AddTransient<ShellPage>();
                    services.AddTransient<ShellViewModel>();
                    services.AddTransient<SettingsPageViewModel>();
                    services.AddTransient<SettingsPage>();
                    services.AddTransient<MainPageViewModel>();
                    services.AddTransient<MainPage>();
                    services.AddTransient<CoinPageViewModel>();
                    services.AddTransient<CoinPage>();

                    // Configuration
                    services.Configure<LocalSettingsOptions>(context.Configuration.GetSection(nameof(LocalSettingsOptions)));
                })
            .Build();

        UnhandledException += App_UnhandledException;
    }

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        Logger.Log(e?.ToString() ?? "Unknown exception", LogLevel.Error);
    }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        await GetService<IActivationService>().ActivateAsync(args);
    }
}
