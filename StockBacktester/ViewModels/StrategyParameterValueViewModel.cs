using CommunityToolkit.Mvvm.ComponentModel;
using StockBacktester.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StockBacktester.ViewModels;

public partial class StrategyParameterValueViewModel : ObservableObject {
  [ObservableProperty]
  private string name;
  [ObservableProperty]
  private string displayName;
  [ObservableProperty]
  private StrategyParameterType type;
  [ObservableProperty]
  private string stringValue;
  [ObservableProperty]
  private DateTime dateTimeValue;
  [ObservableProperty]
  private int intValue;
  [ObservableProperty]
  private double doubleValue;
  [ObservableProperty]
  private double doublePercentValue;
  [ObservableProperty]
  private bool boolValue;

  public StrategyParameterValueViewModel(string name, StrategyParameterType type, object value) {
    Name = name;
    DisplayName = name;
    Type = type;
    SetValue(value);
  }

  public StrategyParameterValueViewModel(string name, string displayName, StrategyParameterType type, object value) {
    Name = name;
    DisplayName = displayName;
    Type = type;
    SetValue(value);
  }

  public void SetValue(object value) {
    if (value == null) return;

    switch (Type) {
      case StrategyParameterType.String:
        if (value is not string) return;
        StringValue = (string)value;
        break;
      case StrategyParameterType.DateTime:
        if (value is not DateTime) return;
        DateTimeValue = (DateTime)value;
        break;
      case StrategyParameterType.Int:
        if (value is not int) return;
        IntValue = (int)value;
        break;
      case StrategyParameterType.Double:
        if (value is not double) return;
        DoubleValue = (double)value;
        break;
      case StrategyParameterType.DoublePercent:
        if (value is not double) return;
        DoublePercentValue = (double)value;
        break;
      case StrategyParameterType.Bool:
        if (value is not bool) return;
        BoolValue = (bool)value;
        break;
    }
  }

  public object GetValue() {
    switch (Type) {
      case StrategyParameterType.String:
        return StringValue;
      case StrategyParameterType.DateTime:
        return DateTimeValue;
      case StrategyParameterType.Int:
        return IntValue;
      case StrategyParameterType.Double:
        return DoubleValue;
      case StrategyParameterType.DoublePercent:
        return DoublePercentValue;
      case StrategyParameterType.Bool:
        return BoolValue;
    }
    return null;
  }
}
