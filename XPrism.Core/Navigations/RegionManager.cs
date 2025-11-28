using System.Windows;
using XPrism.Core.DI;
using XPrism.Core.Navigations;

namespace XPrism.Core.Navigations;

public class RegionManager : IRegionManager
{
    private readonly NavigationRegistry _navigationRegistry;
    public IContainerProvider _container { get; }

    /// <summary>
    /// 区域集合
    /// </summary>
    public IRegionCollection Regions { get; }

    public RegionManager()
    {
        _container = ContainerLocator.Container.GetIContainerExtension();
        _navigationRegistry = new NavigationRegistry(_container);
        Regions = new RegionCollection();
    }


    public Type? GetDataContext(string viewName)
    {
        return _navigationRegistry.GetViewModelType(viewName);
    }

    /// <summary>
    /// 注册视图到区域
    /// </summary>
    public void RegisterViewWithRegion(string regionName, Type viewType, string? viewName = null)
    {
        // 确保区域存在
        if (!Regions.ContainsRegionWithName(regionName))
        {
            var region = new Region(regionName, _container);
            Regions.Add(regionName, region);
        }

        var region1 = Regions.GetRegion(regionName);
        region1.RegisterView(viewName ?? viewType.Name, viewType);
        ContainerLocator.Container.RegisterSingleton(viewType, viewType);
    }

    /// <summary>
    /// 注册视图到区域
    /// </summary>
    public void RegisterViewWithRegion<TView>(string regionName, string? viewName = null)
        where TView : FrameworkElement
    {
        RegisterViewWithRegion(regionName, typeof(TView), viewName);
        ContainerLocator.Container.RegisterSingleton<TView>();
    }

    /// <summary>
    /// 注册视图和ViewModel到导航系统
    /// </summary>
    public void RegisterForNavigation<TView, TViewModel>(string regionName, string? viewName = null)
        where TView : FrameworkElement
        where TViewModel : class
    {
        // 注册到容器
        ContainerLocator.Container.RegisterSingleton<TView>();
        ContainerLocator.Container.RegisterSingleton<TViewModel>();

        // 注册到导航系统
        _navigationRegistry.RegisterView<TView, TViewModel>(viewName ?? nameof(TView));

        // 可以选择自动注册到默认区域
        RegisterViewWithRegion<TView>(regionName, viewName: viewName);
    }

    public void RegisterForNavigation(Type view, Type viewModel, string regionName, string viewName)
    {
        ContainerLocator.Container.RegisterSingleton(view, view);
        ContainerLocator.Container.RegisterSingleton(viewModel, viewModel);

        // 注册到导航系统
        _navigationRegistry.RegisterView(view, viewModel, viewName);
        RegisterViewWithRegion(regionName, view, viewName);
    }

    /// <summary>
    /// 获取区域
    /// </summary>
    public IRegion GetRegion(string regionName)
    {
        return Regions.GetRegion(regionName);
    }

    /// <summary>
    /// 移除区域
    /// </summary>
    public void RemoveRegion(string regionName)
    {
        Regions.Remove(regionName);
    }
}