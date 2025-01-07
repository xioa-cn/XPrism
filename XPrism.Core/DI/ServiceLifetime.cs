namespace XPrism.Core.DI;

/// <summary>
/// 服务生命周期枚举
/// </summary>
public enum ServiceLifetime
{
    /// <summary>
    /// 瞬时生命周期：每次请求都创建新的实例
    /// </summary>
    Transient,

    /// <summary>
    /// 作用域生命周期：在同一个作用域内共享同一个实例
    /// </summary>
    Scoped,

    /// <summary>
    /// 单例生命周期：整个应用程序共享同一个实例
    /// </summary>
    Singleton
}