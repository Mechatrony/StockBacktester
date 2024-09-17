using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kis;
using Kis.Dto;
using Kis.Enums;
using StockBacktester.Models;

namespace StockBacktester.ViewModels;

public partial class MainViewModel : ObservableRecipient {
  public MainViewModel() {
    string keyPath = @"C:\LicenseKeys\KisKey.txt";
    string[] lines = File.ReadAllLines(keyPath);
    string account = lines[0].Split(' ').Last();
    string appKey = lines[1].Split(' ').Last();
    string appSecret = lines[2].Split(' ').Last();

    client = new KisClient(true, appKey, appSecret, account);
  }

  private readonly KisClient client;

  [RelayCommand]
  private async Task Test() {
    if (!await client.CheckAccessToken()) {
      Logger.Log("Failed to get AccessToken", LogLevel.Error);
      return;
    }
    
    string stockCode = StockCodes.삼성전자;
    DateTime startDate = new DateTime(2020, 1, 1);
    DateTime endDate = DateTime.Now - TimeSpan.FromDays(7);
    var ohlcvs = await client.FetchOhlcvAsync(
      stockCode, startDate, endDate, Timeframe.OneDay);

    if (ohlcvs != null) {
      Logger.Log($"Got ohlcvs" +
        $" from {ohlcvs.First().DateTime:yyyy-MM-dd}" +
        $" to {ohlcvs.Last().DateTime:yyyy-MM-dd}" +
        $", Count: {ohlcvs.Length}");

      Logger.Log(ohlcvs.First().ToString());
      Logger.Log(ohlcvs.Last().ToString());
    } else {
      Logger.Log("Failed to fetch current price", LogLevel.Error);
    }
  }
}
