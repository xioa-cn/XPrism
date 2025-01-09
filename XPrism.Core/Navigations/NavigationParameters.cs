using System.Collections;

namespace XPrism.Core.Navigations;

/// <summary>
/// 导航参数实现
/// </summary>
public class NavigationParameters : INavigationParameters
{
    private readonly Dictionary<string, object> _parameters = new();

    /// <summary>
    /// 构造函数
    /// </summary>
    public NavigationParameters()
    {
    }

    /// <summary>
    /// 从查询字符串构造
    /// </summary>
    /// <param name="query">查询字符串</param>
    public NavigationParameters(string query)
    {
        if (string.IsNullOrEmpty(query))
            return;

        // 移除开头的 ? 号
        query = query.TrimStart('?');

        // 解析查询字符串
        var pairs = query.Split('&');
        foreach (var pair in pairs)
        {
            var parts = pair.Split('=');
            if (parts.Length != 2) continue;

            var key = Uri.UnescapeDataString(parts[0]);
            var value = Uri.UnescapeDataString(parts[1]);
            _parameters.Add(key, value);
        }
    }

    /// <summary>
    /// 添加参数
    /// </summary>
    public void Add(string key, object value)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException(nameof(key));

        _parameters[key] = value;
    }

    /// <summary>
    /// 获取参数值
    /// </summary>
    public T? GetValue<T>(string key)
    {
        if (!_parameters.TryGetValue(key, out var value))
            return default;

        if (value is T typedValue)
            return typedValue;

        try
        {
            // 尝试类型转换
            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// 获取所有参数键
    /// </summary>
    public IEnumerable<string> Keys => _parameters.Keys;

    /// <summary>
    /// 获取所有参数值
    /// </summary>
    public IEnumerable<object> Values => _parameters.Values;

    /// <summary>
    /// 获取参数数量
    /// </summary>
    public int Count => _parameters.Count;

    /// <summary>
    /// 是否包含指定键
    /// </summary>
    public bool ContainsKey(string key) => _parameters.ContainsKey(key);

    /// <summary>
    /// 移除指定参数
    /// </summary>
    public bool Remove(string key) => _parameters.Remove(key);

    /// <summary>
    /// 清空所有参数
    /// </summary>
    public void Clear() => _parameters.Clear();

    /// <summary>
    /// 获取枚举器
    /// </summary>
    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => 
        _parameters.GetEnumerator();

    /// <summary>
    /// 获取枚举器
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// 转换为查询字符串
    /// </summary>
    public override string ToString()
    {
        var pairs = _parameters.Select(p => 
            $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value.ToString() ?? string.Empty)}");
        return string.Join("&", pairs);
    }
}