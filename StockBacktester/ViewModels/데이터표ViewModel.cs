using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

using StockBacktester.Contracts.ViewModels;
using StockBacktester.Core.Contracts.Services;
using StockBacktester.Core.Models;

namespace StockBacktester.ViewModels;

public partial class 데이터표ViewModel : ObservableRecipient, INavigationAware
{
    private readonly ISampleDataService _sampleDataService;

    public ObservableCollection<SampleOrder> Source { get; } = new ObservableCollection<SampleOrder>();

    public 데이터표ViewModel(ISampleDataService sampleDataService)
    {
        _sampleDataService = sampleDataService;
    }

    public async void OnNavigatedTo(object parameter)
    {
        Source.Clear();

        // TODO: Replace with real data.
        var data = await _sampleDataService.GetGridDataAsync();

        foreach (var item in data)
        {
            Source.Add(item);
        }
    }

    public void OnNavigatedFrom()
    {
    }
}
