using XPrism.Core.DI;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace XPrism.Core.Navigations;

/// <summary>
/// 区域实现
/// </summary>
public class Region : IRegion {
    private readonly Dictionary<string, Type> _viewTypes = new();
    private readonly IContainerProvider _container;
    private object? _currentView;

    public event EventHandler<ViewChangedEventArgs>? CurrentViewChanged;

    /// <summary>
    /// 获取所有注册的视图类型
    /// </summary>
    public IEnumerable<Type> ViewTypes => _viewTypes.Values;

    public string Name { get; }

    public object? CurrentView {
        get => _currentView;
        private set
        {
            if (_currentView != value)
            {
                var oldView = _currentView;
                _currentView = value;
                OnCurrentViewChanged(oldView, value);
            }
        }
    }


    public Region(string name, IContainerProvider container) {
        Name = name;
        _container = container;
    }

    public async Task<bool> NavigateAsync(string viewName, INavigationParameters parameters, Type? vmType = null) {
        try
        {
            // 检查视图是否已注册
            if (!_viewTypes.TryGetValue(viewName, out var viewType))
            {
                Debug.WriteLine($"View {viewName} not found in region {Name}");
                return false;
            }

            // 创建视图实例
            var view = _container.Resolve(viewType);
            if (view == null)
            {
                Debug.WriteLine($"Failed to create view instance for {viewName}");
                return false;
            }


            if (vmType != null && view is FrameworkElement frameworkElement)
            {
                object? currentViewModel = null;
                currentViewModel = _container.Resolve(vmType);

                frameworkElement.DataContext = currentViewModel;
            }


            // 处理导航感知
            if (view is INavigationAware navigationAware)
            {
                // 检查是否可以导航
                if (!await navigationAware.CanNavigateToAsync(parameters))
                {
                    Debug.WriteLine($"Navigation to {viewName} was rejected by the view");
                    return false;
                }

                // 触发导航事件
                await navigationAware.OnNavigatingToAsync(parameters);
            }

            // 更新当前视图
            CurrentView = view;
            Debug.WriteLine($"Successfully navigated to {viewName} in region {Name}");
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Navigation failed: {ex.Message}");
            return false;
        }
    }

    public Type? GetViewType(string viewName) {
        if (_viewTypes.TryGetValue(viewName, out var viewType))
        {
            return viewType;
        }

        Debug.WriteLine($"View {viewName} not found in region {Name}");

        return null;
    }

    public void ResetView(string viewName) {
        if (_viewTypes.TryGetValue(viewName, out var viewType))
        {
            XPrismIoc.ResetXPrismModel(viewType);
        }
        else
        {
            Debug.WriteLine($"View {viewName} not found in region {Name}");
        }
    }

    public void ResetVm(Type? vmTypeName) {
        if (vmTypeName != null)
            XPrismIoc.ResetXPrismModel(vmTypeName);
    }

    public void ResetViews(string viewName, Type vmType) {
        ResetView(viewName);
        ResetVm(vmType);
    }

    public void RegisterView(string viewName, Type viewType) {
        if (string.IsNullOrEmpty(viewName))
            throw new ArgumentNullException(nameof(viewName));

        if (viewType == null)
            throw new ArgumentNullException(nameof(viewType));

        _viewTypes[viewName] = viewType;
        Debug.WriteLine($"View {viewName} registered in region {Name}");
    }


    public void RemoveView(string viewName) {
        //TODO 移除视图是为了做什么？ 没有什么意义
    }

    protected virtual void OnCurrentViewChanged(object? oldView, object? newView) {
        CurrentViewChanged?.Invoke(this, new ViewChangedEventArgs(oldView, newView));
        Debug.WriteLine($"View changed in region {Name}");
    }
}