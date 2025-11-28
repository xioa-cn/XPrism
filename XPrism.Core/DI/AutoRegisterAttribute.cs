namespace XPrism.Core.DI
{
    /// <summary>
    /// 标记需要自动注册的类型
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AutoRegisterAttribute : System.Attribute
    {
        /// <summary>
        /// 服务生命周期
        /// </summary>
        public ServiceLifetime Lifetime { get; }

        /// <summary>
        /// 指定要注册的服务类型（接口）
        /// 如果为null，则自动查找实现的接口
        /// </summary>
        public Type? ServiceType { get; }

        /// <summary>
        /// 服务名称
        /// 如果不为null，则使用命名注册
        /// </summary>
        public string? ServiceName { get; }

        /// <summary>
        /// 初始化自动注册特性
        /// </summary>
        /// <param name="lifetime">服务生命周期</param>
        /// <param name="serviceName">服务名称（可选）</param>
        public AutoRegisterAttribute(ServiceLifetime lifetime = ServiceLifetime.Singleton, string? serviceName = null)
        {
            Lifetime = lifetime;
            ServiceName = serviceName;
        }

        /// <summary>
        /// 初始化自动注册特性
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="lifetime">服务生命周期</param>
        /// <param name="serviceName">服务名称（可选）</param>
        public AutoRegisterAttribute(Type serviceType, ServiceLifetime lifetime = ServiceLifetime.Transient, string? serviceName = null)
        {
            ServiceType = serviceType;
            Lifetime = lifetime;
            ServiceName = serviceName;
        }
    }
} 