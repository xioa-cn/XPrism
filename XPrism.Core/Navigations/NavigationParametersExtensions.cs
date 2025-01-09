namespace XPrism.Core.Navigations;

/// <summary>
/// 导航参数扩展方法
/// </summary>
public static class NavigationParametersExtensions
{
    /// <summary>
    /// 添加参数并返回参数集合（支持链式调用）
    /// </summary>
    public static NavigationParameters AddAndReturn(
        this NavigationParameters parameters,
        string key,
        object value)
    {
        parameters.Add(key, value);
        return parameters;
    }

    /// <summary>
    /// 尝试获取参数值
    /// </summary>
    public static bool TryGetValue<T>(
        this INavigationParameters parameters,
        string key,
        out T? value)
    {
        value = parameters.GetValue<T>(key);
        return value != null;
    }
}