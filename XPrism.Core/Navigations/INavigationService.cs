namespace XPrism.Core.Navigations;

/// <summary>
/// 导航服务接口
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// 导航到指定页面
    /// </summary>
    /// <param name="path">导航路径 (格式: "RegionName/ViewName")</param>
    /// <param name="parameters">导航参数</param>
    Task<bool> NavigateAsync(string path, INavigationParameters? parameters = null);

    /// <summary>
    /// 获取导航界面
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    Task<object?> FetchNavigateViewAsync(string path);
    
    /// <summary>
    /// 返回上一页
    /// </summary>
    Task<bool> GoBackAsync(string regionName);

    /// <summary>
    /// 是否可以返回
    /// </summary>
    bool CanGoBack(string regionName);

    /// <summary>
    /// 当前视图
    /// </summary>
    object? CurrentView(string regionName);
    
    /// <summary>
    /// 重置页面
    /// </summary>
    /// <param name="viewNames"></param>
    void ResetView(string viewNames);
    /// <summary>
    /// 重置Vm
    /// </summary>
    /// <param name="viewNames"></param>
    void ResetVm(string viewNames);
    /// <summary>
    /// 重置页面和Vm
    /// </summary>
    /// <param name="viewNames"></param>
    void ResetViews(string viewNames);
}