using Meziantou.Framework.WPF.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;
using Backtester.Models;
using System.Collections.Specialized;

namespace Backtester.Behaviors;

public class AddingListBoxBehavior : Behavior<ListBox>
{
    protected override void OnAttached()
    {
        base.OnAttached();

        AssociatedObject.DataContextChanged += AssociatedObject_DataContextChanged;
    }

    private void AssociatedObject_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    {
        ((IReadOnlyObservableCollection<LogEntry>)AssociatedObject.ItemsSource).CollectionChanged += ItemsSource_CollectionChanged;
    }

    protected override void OnDetaching()
    {
        ((IReadOnlyObservableCollection<LogEntry>)AssociatedObject.ItemsSource).CollectionChanged -= ItemsSource_CollectionChanged;
        AssociatedObject.DataContextChanged -= AssociatedObject_DataContextChanged;

        base.OnDetaching();
    }

    private void ItemsSource_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (AssociatedObject?.Items.Count > 0)
        {
            object item = AssociatedObject.Items.Last();
            AssociatedObject.ScrollIntoView(item);
        }
    }
}
