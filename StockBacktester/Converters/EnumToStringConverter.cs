using Microsoft.UI.Xaml.Data;

namespace StockBacktester.Converters;

public class EnumToStringConverter : IValueConverter {
  public object Convert(object value, Type targetType, object parameter, string language) {
    return value?.ToString();
  }

  public object ConvertBack(object value, Type targetType, object parameter, string language) {
    throw new NotImplementedException();
  }
}
