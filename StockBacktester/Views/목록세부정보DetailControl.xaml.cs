using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using StockBacktester.Core.Models;

namespace StockBacktester.Views;

public sealed partial class 목록세부정보DetailControl : UserControl
{
    public SampleOrder? ListDetailsMenuItem
    {
        get => GetValue(ListDetailsMenuItemProperty) as SampleOrder;
        set => SetValue(ListDetailsMenuItemProperty, value);
    }

    public static readonly DependencyProperty ListDetailsMenuItemProperty = DependencyProperty.Register("ListDetailsMenuItem", typeof(SampleOrder), typeof(목록세부정보DetailControl), new PropertyMetadata(null, OnListDetailsMenuItemPropertyChanged));

    public 목록세부정보DetailControl()
    {
        InitializeComponent();
    }

    private static void OnListDetailsMenuItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is 목록세부정보DetailControl control)
        {
            control.ForegroundElement.ChangeView(0, 0, 1);
        }
    }
}
