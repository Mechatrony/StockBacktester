using Microsoft.Extensions.Options;
using StockBacktester.Contracts.Services;
using StockBacktester.Helpers;
using StockBacktester.Models;
using Windows.Storage;

namespace StockBacktester.Services;

public class LocalSettingsService : ILocalSettingsService
{
    private const string defaultApplicationDataFolder = "StockBacktester/ApplicationData";
    private const string defaultLocalSettingsFile = "LocalSettings.json";

    private readonly FileService fileService;
    private readonly LocalSettingsOptions options;

    private readonly string localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    private readonly string applicationDataFolder;
    private readonly string localsettingsFile;

    private IDictionary<string, object> settings;

    private bool isInitialized;

    public LocalSettingsService(FileService fileService, IOptions<LocalSettingsOptions> options)
    {
        this.fileService = fileService;
        this.options = options.Value;

        applicationDataFolder = Path.Combine(localApplicationData, this.options.ApplicationDataFolder ?? defaultApplicationDataFolder);
        localsettingsFile = this.options.LocalSettingsFile ?? defaultLocalSettingsFile;

        settings = new Dictionary<string, object>();
    }

    private async Task InitializeAsync()
    {
        if (isInitialized)
            return;

        // TODO: Is async needed?
        settings =
            await Task.Run(() => fileService.ReadJson<IDictionary<string, object>?>(applicationDataFolder, localsettingsFile))
            ?? new Dictionary<string, object>();
        isInitialized = true;
    }

    public async Task<T?> ReadSettingAsync<T>(string key)
    {
        if (RuntimeHelper.IsMSIX)
        {
            if (ApplicationData.Current.LocalSettings.Values.TryGetValue(key, out var obj))
            {
                return await Json.ToObjectAsync<T>((string)obj);
            }
        }
        else
        {
            await InitializeAsync();

            if (settings != null && settings.TryGetValue(key, out var obj))
            {
                return await Json.ToObjectAsync<T>((string)obj);
            }
        }

        return default;
    }

    public async Task SaveSettingAsync<T>(string key, T value)
    {
        if (RuntimeHelper.IsMSIX)
        {
            ApplicationData.Current.LocalSettings.Values[key] = await Json.StringifyAsync(value);
        }
        else
        {
            await InitializeAsync();

            settings[key] = await Json.StringifyAsync(value);

            await Task.Run(() => fileService.SaveJson(applicationDataFolder, localsettingsFile, settings));
        }
    }
}
