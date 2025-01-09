namespace XPrism.Core.Navigations;

/// <summary>
/// 导航感知基类
/// </summary>
public abstract class NavigationAwareBase : INavigationAware
{
    /// <summary>
    /// 导航到此页面时触发
    /// </summary>
    public virtual Task OnNavigatingToAsync(INavigationParameters parameters)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// 离开此页面时触发
    /// </summary>
    public virtual Task OnNavigatingFromAsync(INavigationParameters parameters)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// 确认是否可以导航到此页面
    /// </summary>
    public virtual Task<bool> CanNavigateToAsync(INavigationParameters parameters)
    {
        return Task.FromResult(true);
    }

    /// <summary>
    /// 确认是否可以离开此页面
    /// </summary>
    public virtual Task<bool> CanNavigateFromAsync(INavigationParameters parameters)
    {
        return Task.FromResult(true);
    }
}