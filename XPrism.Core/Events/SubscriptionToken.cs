namespace XPrism.Core.Events;

/// <summary>
/// 表示事件订阅的令牌，用于管理订阅的生命周期
/// </summary>
public class SubscriptionToken : IDisposable
{
    private readonly Action _unsubscribeAction;
    
    /// <summary>
    /// 订阅的类型（同步/异步，有返回值/无返回值）
    /// </summary>
    public SubscriptionType SubscriptionType { get; }

    /// <summary>
    /// 唯一标识符
    /// </summary>
    public Guid TokenId { get; } = Guid.NewGuid();

    /// <summary>
    /// 用户自定义的标识对象
    /// </summary>
    public object? Tag { get; }

    /// <summary>
    /// 初始化订阅令牌
    /// </summary>
    /// <param name="unsubscribeAction">取消订阅的委托</param>
    /// <param name="subscriptionType">订阅类型</param>
    /// <param name="tag">用户自定义的标识对象</param>
    public SubscriptionToken(Action unsubscribeAction, SubscriptionType subscriptionType, object? tag = null)
    {
        _unsubscribeAction = unsubscribeAction;
        SubscriptionType = subscriptionType;
        Tag = tag;
    }

    /// <summary>
    /// 释放资源，取消订阅
    /// </summary>
    public void Dispose()
    {
        _unsubscribeAction();
    }
}

