using CommunityToolkit.Mvvm.ComponentModel;
using Backtester.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Backtester.ViewModels;

public partial class StrategyParameterValueViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string Name { get; set; }
    [ObservableProperty]
    public partial string DisplayName { get; set; }
    [ObservableProperty]
    public partial StrategyParameterType Type { get; set; }
    [ObservableProperty]
    public partial string StringValue { get; set; }
    [ObservableProperty]
    public partial DateTime DateTimeValue { get; set; }
    [ObservableProperty]
    public partial int IntValue { get; set; }
    [ObservableProperty]
    public partial double DoubleValue { get; set; }
    [ObservableProperty]
    public partial double DoublePercentValue { get; set; }
    [ObservableProperty]
    public partial bool BoolValue { get; set; }

    public StrategyParameterValueViewModel(string name, StrategyParameterType type, object value)
    {
        Name = name;
        DisplayName = name;
        Type = type;
        SetValue(value);
    }

    public StrategyParameterValueViewModel(string name, string displayName, StrategyParameterType type, object value)
    {
        Name = name;
        DisplayName = displayName;
        Type = type;
        SetValue(value);
    }

    public void SetValue(object value)
    {
        if (value == null) return;

        switch (Type)
        {
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

    public object GetValue()
    {
        return Type switch
        {
            StrategyParameterType.String => StringValue,
            StrategyParameterType.DateTime => DateTimeValue,
            StrategyParameterType.Int => IntValue,
            StrategyParameterType.Double => DoubleValue,
            StrategyParameterType.DoublePercent => DoublePercentValue,
            StrategyParameterType.Bool => BoolValue,
            _ => throw new NotImplementedException(),
        };
    }
}
