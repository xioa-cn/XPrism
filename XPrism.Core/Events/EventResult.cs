namespace XPrism.Core.Events;

/// <summary>
/// 事件执行结果
/// </summary>
public class EventResult
{
    private readonly object? _value;

    public EventResult(object? value)
    {
        _value = value;
    }

    /// <summary>
    /// 获取指定类型的返回值
    /// </summary>
    public T? GetValue<T>()
    {
        return _value == null ? default : (T)_value;
    }
} 