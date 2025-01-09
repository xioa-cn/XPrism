using XPrism.Core.Navigations;

public class NavigationContext
{
    /// <summary>
    /// 导航参数
    /// </summary>
    public INavigationParameters Parameters { get; }

    /// <summary>
    /// 导航源
    /// </summary>
    public object? Source { get; }

    /// <summary>
    /// 导航目标
    /// </summary>
    public object? Target { get; }

    /// <summary>
    /// 导航路径
    /// </summary>
    public string NavigationPath { get; }

    public NavigationContext(
        string navigationPath,
        INavigationParameters parameters,
        object? source = null,
        object? target = null)
    {
        NavigationPath = navigationPath;
        Parameters = parameters;
        Source = source;
        Target = target;
    }
}