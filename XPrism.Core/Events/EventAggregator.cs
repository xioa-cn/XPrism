namespace XPrism.Core.Events;

/// <summary>
/// 事件聚合器的实现，管理所有事件实例
/// </summary>
public class EventAggregator : IEventAggregator
{
    /// <summary>
    /// 存储所有事件实例的字典
    /// </summary>
    private readonly Dictionary<Type, EventBase> _events = new();
    
    /// <summary>
    /// 用于线程同步的锁对象
    /// </summary>
    private readonly object _lockObject = new();

    /// <summary>
    /// 获取或创建指定类型的事件实例
    /// </summary>
    /// <typeparam name="TEventType">事件类型</typeparam>
    /// <returns>事件实例</returns>
    public TEventType GetEvent<TEventType>() where TEventType : EventBase, new()
    {
        lock (_lockObject)
        {
            var eventType = typeof(TEventType);

            if (_events.TryGetValue(eventType, out var existingEvent)) return (TEventType)existingEvent;
            existingEvent = new TEventType();
            _events[eventType] = existingEvent;

            return (TEventType)existingEvent;
        }
    }
} 