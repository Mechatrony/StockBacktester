using Backtester.Enums;
using Backtester.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Backtester.TemplateSelectors;

public class StrategyParameterTemplateSelector : DataTemplateSelector
{
    public DataTemplate? StringTemplate { get; set; }
    public DataTemplate? DateTimeTemplate { get; set; }
    public DataTemplate? IntTemplate { get; set; }
    public DataTemplate? DoubleTemplate { get; set; }
    public DataTemplate? DoublePercentTemplate { get; set; }
    public DataTemplate? BoolTemplate { get; set; }

    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        return ((StrategyParameterValueViewModel)item).Type switch
        {
            StrategyParameterType.String => StringTemplate,
            StrategyParameterType.DateTime => DateTimeTemplate,
            StrategyParameterType.Int => IntTemplate,
            StrategyParameterType.Double => DoubleTemplate,
            StrategyParameterType.DoublePercent => DoublePercentTemplate,
            StrategyParameterType.Bool => BoolTemplate,
            _ => base.SelectTemplate(item, container),
        };
    }
}
