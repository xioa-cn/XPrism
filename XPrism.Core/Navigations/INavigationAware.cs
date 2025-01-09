namespace XPrism.Core.Navigations;

/// <summary>
/// 导航感知接口
/// </summary>
public interface INavigationAware
{
    /// <summary>
    /// 导航到此页面时触发
    /// </summary>
    /// <param name="parameters">导航参数</param>
    Task OnNavigatingToAsync(INavigationParameters parameters);

    /// <summary>
    /// 离开此页面时触发
    /// </summary>
    /// <param name="parameters">导航参数</param>
    Task OnNavigatingFromAsync(INavigationParameters parameters);

    /// <summary>
    /// 确认是否可以导航到此页面
    /// </summary>
    /// <param name="parameters">导航参数</param>
    /// <returns>是否可以导航</returns>
    Task<bool> CanNavigateToAsync(INavigationParameters parameters);

    /// <summary>
    /// 确认是否可以离开此页面
    /// </summary>
    /// <param name="parameters">导航参数</param>
    /// <returns>是否可以离开</returns>
    Task<bool> CanNavigateFromAsync(INavigationParameters parameters);
}