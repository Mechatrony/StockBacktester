using CommunityToolkit.Mvvm.ComponentModel;
using Backtester.Enums;

namespace Backtester.ViewModels;

public partial class StrategyParameterViewModel : ObservableObject
{
    [ObservableProperty]
    private StrategyParameterValueViewModel value;

    public StrategyParameterViewModel(string name, object value, StrategyParameterType type = StrategyParameterType.None)
    {
        if (value is string stringValue)
        {
            Value = new(name, StrategyParameterType.String, stringValue);
        }
        else if (value is DateTime dateTimeValue)
        {
            Value = new(name, StrategyParameterType.DateTime, dateTimeValue);
        }
        else if (value is int intValue)
        {
            Value = new(name, StrategyParameterType.Int, intValue);
        }
        else if (value is double doubleValue)
        {
            if (type == StrategyParameterType.DoublePercent)
            {
                Value = new(name, StrategyParameterType.DoublePercent, doubleValue);
            }
            else
            {
                Value = new(name, StrategyParameterType.Double, doubleValue);
            }
        }
        else if (value is bool boolValue)
        {
            Value = new(name, StrategyParameterType.Bool, boolValue);
        }
    }

    public StrategyParameterViewModel(string name, string displayName, object value, StrategyParameterType type = StrategyParameterType.None)
    {
        if (value is string stringValue)
        {
            Value = new(name, displayName, StrategyParameterType.String, stringValue);
        }
        else if (value is DateTime dateTimeValue)
        {
            Value = new(name, displayName, StrategyParameterType.DateTime, dateTimeValue);
        }
        else if (value is int intValue)
        {
            Value = new(name, displayName, StrategyParameterType.Int, intValue);
        }
        else if (value is double doubleValue)
        {
            if (type == StrategyParameterType.DoublePercent)
            {
                Value = new(name, displayName, StrategyParameterType.DoublePercent, doubleValue);
            }
            else
            {
                Value = new(name, displayName, StrategyParameterType.Double, doubleValue);
            }
        }
        else if (value is bool boolValue)
        {
            Value = new(name, displayName, StrategyParameterType.Bool, boolValue);
        }
    }

    public object GetValue()
    {
        return Value.GetValue();
    }
}
