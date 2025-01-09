namespace XPrism.Core.Navigations;

/// <summary>
/// 导航参数接口
/// </summary>
public interface INavigationParameters : IEnumerable<KeyValuePair<string, object>> {
    /// <summary>
    /// 添加参数
    /// </summary>
    /// <param name="key">参数键</param>
    /// <param name="value">参数值</param>
    void Add(string key, object value);

    /// <summary>
    /// 获取参数值
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    /// <param name="key">参数键</param>
    /// <returns>参数值</returns>
    T? GetValue<T>(string key);

    /// <summary>
    /// 获取所有参数键
    /// </summary>
    IEnumerable<string> Keys { get; }

    /// <summary>
    /// 获取所有参数值
    /// </summary>
    IEnumerable<object> Values { get; }

    /// <summary>
    /// 获取参数数量
    /// </summary>
    int Count { get; }

    /// <summary>
    /// 是否包含指定键
    /// </summary>
    bool ContainsKey(string key);

    /// <summary>
    /// 移除指定参数
    /// </summary>
    bool Remove(string key);

    /// <summary>
    /// 清空所有参数
    /// </summary>
    void Clear();
}