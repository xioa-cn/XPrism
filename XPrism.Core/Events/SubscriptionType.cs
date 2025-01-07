namespace XPrism.Core.Events;

/// <summary>
/// 订阅类型
/// </summary>
public enum SubscriptionType
{
    /// <summary>
    /// 同步无返回值
    /// </summary>
    Sync,
    
    /// <summary>
    /// 同步有返回值
    /// </summary>
    SyncWithResult,
    
    /// <summary>
    /// 异步无返回值
    /// </summary>
    Async,
    
    /// <summary>
    /// 异步有返回值
    /// </summary>
    AsyncWithResult
} 