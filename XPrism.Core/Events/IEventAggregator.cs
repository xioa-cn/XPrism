namespace XPrism.Core.Events;

/// <summary>
/// 定义事件聚合器的接口，用于获取特定类型的事件
/// </summary>
public interface IEventAggregator
{
    /// <summary>
    /// 获取指定类型的事件实例
    /// </summary>
    /// <typeparam name="TEventType">事件类型，必须继承自EventBase且可实例化</typeparam>
    /// <returns>事件实例</returns>
    TEventType GetEvent<TEventType>() where TEventType : EventBase, new();
} 