namespace XPrism.Core.DI;

/// <summary>
/// 服务描述符，用于存储服务注册信息
/// </summary>
internal class ServiceDescriptor {
    /// <summary>
    /// 服务类型
    /// </summary>
    public Type ServiceType { get; }

    /// <summary>
    /// 实现类型
    /// </summary>
    public Type ImplementationType { get; }

    /// <summary>
    /// 生命周期类型
    /// </summary>
    public ServiceLifetime Lifetime { get; }

    /// <summary>
    /// 单例实例
    /// </summary>
    public object? Instance { get; }

    /// <summary>
    /// 缓存的实例（用于单例模式）
    /// </summary>
    public object? CachedInstance { get; set; }

    public Action<object>? RegisterAction { get; }

    
    
    public ServiceDescriptor(Type serviceType, Type implementationType, ServiceLifetime lifetime,
        Action<object>? registerAction = null) {
        ServiceType = serviceType;
        ImplementationType = implementationType;
        Lifetime = lifetime;
        RegisterAction = registerAction;
    }

    public ServiceDescriptor(Type serviceType, object? instance, ServiceLifetime lifetime,Action<object>? registerAction = null) {
        ServiceType = serviceType;
        Instance = instance;
        Lifetime = lifetime;
        RegisterAction = registerAction;
    }
}