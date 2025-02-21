using CommunityToolkit.WinUI.Animations;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Backtester.Contracts.Services;
using Backtester.Contracts.ViewModels;
using Backtester.Helpers;
using System.Diagnostics.CodeAnalysis;

namespace Backtester.Services;

// For more information on navigation between pages see
// https://github.com/microsoft/TemplateStudio/blob/main/docs/WinUI/navigation.md
public class NavigationService : INavigationService
{
    private readonly IPageService pageService;
    private object? lastParameterUsed;
    private Frame? frame;

    public event NavigatedEventHandler? Navigated;

    public Frame? Frame
    {
        get
        {
            if (frame == null)
            {
                frame = App.MainWindow.Content as Frame;
                RegisterFrameEvents();
            }
            return frame;
        }
        set
        {
            UnregisterFrameEvents();
            frame = value;
            RegisterFrameEvents();
        }
    }

    [MemberNotNullWhen(true, nameof(Frame), nameof(frame))]
    public bool CanGoBack => Frame != null && Frame.CanGoBack;

    public NavigationService(IPageService pageService)
    {
        this.pageService = pageService;
    }

    private void RegisterFrameEvents()
    {
        if (frame != null)
        {
            frame.Navigated += OnNavigated;
        }
    }

    private void UnregisterFrameEvents()
    {
        if (frame != null)
        {
            frame.Navigated -= OnNavigated;
        }
    }

    public bool GoBack()
    {
        if (!CanGoBack)
            return false;

        object? fromViewModel = frame.GetPageViewModel();
        frame.GoBack();
        if (fromViewModel is INavigationAware navigationAware)
        {
            navigationAware.OnNavigatedFrom();
        }

        return true;
    }

    public bool NavigateTo(string pageKey, object? parameter = null, bool clearNavigation = false)
    {
        if (frame == null)
            return false;

        Type toPageType = pageService.GetPageType(pageKey);

        if (frame.Content?.GetType() != toPageType
            || (parameter != null && !parameter.Equals(lastParameterUsed)))
        {
            frame.Tag = clearNavigation;
            object? fromViewModel = frame.GetPageViewModel();
            bool navigated = frame.Navigate(toPageType, parameter);
            if (navigated)
            {
                lastParameterUsed = parameter;
                if (fromViewModel is INavigationAware navigationAware)
                {
                    navigationAware.OnNavigatedFrom();
                }
            }

            return navigated;
        }

        return false;
    }

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        if (sender is not Frame frame)
            return;

        bool clearNavigation = (bool)frame.Tag;
        if (clearNavigation)
        {
            frame.BackStack.Clear();
        }

        if (frame.GetPageViewModel() is INavigationAware navigationAware)
        {
            navigationAware.OnNavigatedTo(e.Parameter);
        }

        Navigated?.Invoke(sender, e);
    }

    public void SetListDataItemForNextConnectedAnimation(object item)
        => Frame?.SetListDataItemForNextConnectedAnimation(item);
}
