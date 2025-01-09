using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using XPrism.Core.Navigations;

namespace XPrism.Core.Navigations;

public static class RegionManagerProperty {
    private static IRegionManager? _regionManager;

    public static readonly DependencyProperty RegionNameProperty =
        DependencyProperty.RegisterAttached(
            "RegionName",
            typeof(string),
            typeof(RegionManagerProperty),
            new PropertyMetadata(null, OnRegionNameChanged));

    public static void SetRegionManager(IRegionManager regionManager) {
        _regionManager = regionManager;
    }

    public static string GetRegionName(DependencyObject obj) {
        return (string)obj.GetValue(RegionNameProperty);
    }

    public static void SetRegionName(DependencyObject obj, string value) {
        obj.SetValue(RegionNameProperty, value);
    }

    private static void OnRegionNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (_regionManager == null)
        {
            throw new InvalidOperationException("RegionManager has not been initialized.");
        }

        var regionName = e.NewValue as string;
        if (string.IsNullOrEmpty(regionName)) return;

        // 根据控件类型创建不同的容器
        if (d is ContentControl contentControl)
        {
            // 如果是Page类型的视图，使用Frame作为容器
            if (IsPageRegion(regionName))
            {
                var frame = new Frame();
                contentControl.Content = frame;
                EnsureRegionExists(regionName, frame);
            }
            else
            {
                EnsureRegionExists(regionName, contentControl);
            }
        }
    }

    private static bool IsPageRegion(string regionName) {
        // 检查该区域是否包含Page类型的视图
        if (_regionManager?.Regions.ContainsRegionWithName(regionName) == true)
        {
            var region = _regionManager.Regions.GetRegion(regionName);
            if (region is Region r && r.ViewTypes.Any(t => typeof(Page).IsAssignableFrom(t)))
            {
                return true;
            }
        }

        return false;
    }

    private static void EnsureRegionExists(string regionName, object container) {
        if (_regionManager == null) return;

        if (!_regionManager.Regions.ContainsRegionWithName(regionName))
        {
            var region = new Region(regionName, _regionManager._container);

            // 根据容器类型设置不同的内容更新逻辑
            region.CurrentViewChanged += (s, args) =>
            {
                if (container is Frame frame && args.NewView is Page page)
                {
                    frame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
                    frame.Navigate(page);
                }
                else if (container is ContentControl contentControl)
                {
                    contentControl.Content = args.NewView;
                }
            };

            _regionManager.Regions.Add(regionName, region);
        }
        else
        {
            var region = _regionManager.Regions.GetRegion(regionName);
            if (region is Region r)
            {
                r.CurrentViewChanged += (s, args) =>
                {
                    if (container is Frame frame && args.NewView is Page page)
                    {
                        frame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
                        frame.Navigate(page);
                    }
                    else if (container is ContentControl contentControl)
                    {
                        contentControl.Content = args.NewView;
                    }
                };

                // 如果区域已有当前视图，立即设置
                if (r.CurrentView != null)
                {
                    if (container is Frame frame && r.CurrentView is Page page)
                    {
                        frame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
                        frame.Navigate(page);
                    }
                    else if (container is ContentControl contentControl)
                    {
                        contentControl.Content = r.CurrentView;
                    }
                }
            }
        }
    }
}