using CommunityToolkit.Mvvm.ComponentModel;

using StockBacktester.Contracts.ViewModels;
using StockBacktester.Core.Contracts.Services;
using StockBacktester.Core.Models;

namespace StockBacktester.ViewModels;

public partial class 콘텐츠표DetailViewModel : ObservableRecipient, INavigationAware
{
    private readonly ISampleDataService _sampleDataService;

    [ObservableProperty]
    private SampleOrder? item;

    public 콘텐츠표DetailViewModel(ISampleDataService sampleDataService)
    {
        _sampleDataService = sampleDataService;
    }

    public async void OnNavigatedTo(object parameter)
    {
        if (parameter is long orderID)
        {
            var data = await _sampleDataService.GetContentGridDataAsync();
            Item = data.First(i => i.OrderID == orderID);
        }
    }

    public void OnNavigatedFrom()
    {
    }
}
