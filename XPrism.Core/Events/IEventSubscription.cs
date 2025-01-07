namespace XPrism.Core.Events;

/// <summary>
/// 定义事件订阅的接口
/// </summary>
public interface IEventSubscription {
    /// <summary>
    /// 获取订阅是否仍然有效
    /// </summary>
    bool IsActive { get; }

    /// <summary>
    /// 获取事件处理委托（无返回值）
    /// </summary>
    Action<object> Action { get; }

    /// <summary>
    /// 获取事件处理委托（有返回值）
    /// </summary>
    Func<object, object?>? ActionWithResult { get; }

    /// <summary>
    /// 是否是有返回值的订阅
    /// </summary>
    bool HasResult { get; }

    /// <summary>
    /// 获取线程选项
    /// </summary>
    ThreadOption ThreadOption { get; }

    /// <summary>
    /// 获取或设置订阅令牌
    /// </summary>
    SubscriptionToken? SubscriptionToken { get; set; }

    /// <summary>
    /// 检查是否应该处理事件
    /// </summary>
    /// <param name="payload">事件数据</param>
    /// <returns>如果应该处理事件则返回true</returns>
    bool ShouldHandle(object payload);

    /// <summary>
    /// 执行订阅动作并返回结果
    /// </summary>
    object? Execute(object payload);
}