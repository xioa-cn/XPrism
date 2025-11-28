using System.Windows;
using XPrism.Core.DI;

namespace XPrism.Core.Navigations;

/// <summary>
/// 区域管理接口
/// </summary>
public interface IRegionManager {
    public IContainerProvider _container { get; }

    /// <summary>
    /// 获取区域集合
    /// </summary>
    IRegionCollection Regions { get; }


    /// <summary>
    /// 获取视图的DataContext
    /// </summary>
    /// <param name="viewName"></param>
    /// <returns></returns>
    Type? GetDataContext(string viewName);

    /// <summary>
    /// 注册视图到区域
    /// <param name="regionName">区域名称</param>
    /// <param name="viewType">视图类型</param>
    /// <param name="viewName">视图访问名称</param>
    /// </summary>
    void RegisterViewWithRegion(string regionName, Type viewType, string? viewName = null);

    /// <summary>
    /// 注册视图到区域
    /// <param name="regionName">区域名称</param>
    /// <param name="viewName">视图访问名称</param>
    /// </summary>
    void RegisterViewWithRegion<TView>(string regionName, string? viewName = null) where TView : FrameworkElement;

    /// <summary>
    /// 注册视图和ViewModel到导航系统
    /// </summary>
    /// <param name="regionName">区域名称</param>
    /// <param name="viewName">视图访问名称</param>
    /// <typeparam name="TView"></typeparam>
    /// <typeparam name="TViewModel"></typeparam>
    void RegisterForNavigation<TView, TViewModel>(string regionName, string? viewName = null) where TView : FrameworkElement
        where TViewModel : class;

    void RegisterForNavigation(Type view,Type viewModel,string regionName, string viewName);
}