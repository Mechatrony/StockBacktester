using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kis;
using Kis.Dto;
using StockBacktester.Models;

namespace StockBacktester.ViewModels;

public partial class MainViewModel : ObservableRecipient {
  public MainViewModel() {
  }

  [RelayCommand]
  private async Task Test() {
    Logger.Log("Hello");
   
    string keyPath = @"C:\LicenseKeys\KisKey.txt";
    string[] lines = File.ReadAllLines(keyPath);
    string account = lines[0].Split(' ').Last();
    string appKey = lines[1].Split(' ').Last();
    string appSecret = lines[2].Split(' ').Last();

    KisClient client = new KisClient(true, appKey, appSecret, account);

    if (!await client.CheckAccessToken()) {
      Logger.Log("Failed to get AccessToken", LogLevel.Error);
      return;
    }
    
    var dto = await client.주식현재가시세("005930");
    if (dto != null) {
      Logger.Log(dto.ToString());
    } else {
      Logger.Log("Failed to fetch current price", LogLevel.Error);
    }
  }
}
