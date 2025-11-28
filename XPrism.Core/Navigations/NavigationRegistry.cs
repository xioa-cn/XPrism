using System.Diagnostics;
using System.Windows;
using XPrism.Core.DebugLog;
using XPrism.Core.DI;

namespace XPrism.Core.Navigations;

/// <summary>
/// 导航注册器
/// </summary>
public class NavigationRegistry
{
    private readonly Dictionary<string, Type> _viewMappings = new();
    private readonly Dictionary<string, Type> _viewModelMappings = new();
    private readonly IContainerProvider _container;

    public NavigationRegistry(IContainerProvider container)
    {
        _container = container;
    }

    public Type? GetViewModelType(string viewName)
    {
        _viewModelMappings.TryGetValue(viewName, out var viewType);
        return viewType;
    }

    /// <summary>
    /// 注册视图
    /// </summary>
    public void RegisterView<TView>(string viewName) where TView : FrameworkElement
    {
        _viewMappings[viewName] = typeof(TView);
    }

    /// <summary>
    /// 注册视图和ViewModel
    /// </summary>
    public void RegisterView<TView, TViewModel>(string viewName)
        where TView : FrameworkElement
        where TViewModel : class
    {
        _viewMappings[viewName] = typeof(TView);
        _viewModelMappings[viewName] = typeof(TViewModel);
    }

    public void RegisterView(Type view, Type viewModel, string viewName)
    {
        _viewMappings[viewName] = view;
        _viewModelMappings[viewName] = viewModel;
    }

    /// <summary>
    /// 创建视图实例
    /// </summary>
    public object? CreateView(string viewName)
    {
        if (!_viewMappings.TryGetValue(viewName, out var viewType))
            return null;

        // 创建视图实例
        var view = _container.Resolve(viewType);

        // 如果有对应的ViewModel，设置DataContext
        if (view is FrameworkElement element &&
            _viewModelMappings.TryGetValue(viewName, out var viewModelType))
        {
            try
            {
                var viewModel = _container.Resolve(viewModelType);
                element.DataContext = viewModel;
            }
            catch (Exception ex)
            {
                // 可以添加日志记录
                DebugLogger.LogError($"Failed to create ViewModel for {viewName}: {ex.Message}");
            }
        }

        return view;
    }
}