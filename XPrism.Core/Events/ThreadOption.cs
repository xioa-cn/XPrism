namespace XPrism.Core.Events;

/// <summary>
/// 定义事件处理的线程选项
/// </summary>
public enum ThreadOption
{
    /// <summary>
    /// 在发布者的线程上执行
    /// </summary>
    PublisherThread,

    /// <summary>
    /// 在UI线程上执行
    /// </summary>
    UIThread,

    /// <summary>
    /// 在后台线程上执行
    /// </summary>
    BackgroundThread
} 