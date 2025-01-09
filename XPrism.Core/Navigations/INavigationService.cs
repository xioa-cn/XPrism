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
    /// 返回上一页
    /// </summary>
    Task<bool> GoBackAsync();

    /// <summary>
    /// 是否可以返回
    /// </summary>
    bool CanGoBack { get; }

    /// <summary>
    /// 当前视图
    /// </summary>
    object? CurrentView { get; }
}