using XPrism.Core.DI;

namespace XPrism.Core.DataContextWindow {
    /// <summary>
    /// 用于标记视图对应的ViewModel
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class XPrismViewModelAttribute : Attribute {
        public ServiceLifetime Lifetime { get; }

        /// <summary>
        /// ViewModel类型
        /// </summary>
        public Type? ViewModelType { get; }

        /// <summary>
        /// 服务名称（可选）
        /// </summary>
        public string? ServiceName { get; }

        public string? ViewName { get; }

        /// <summary>
        /// 初始化ViewModel特性
        /// </summary>
        /// <param name="viewName">window 注册名称</param>
        /// <param name="viewModelType">ViewModel类型</param>
        /// <param name="serviceName">服务名称（可选）</param>
        /// <param name="lifetime">注册时态</param>
        public XPrismViewModelAttribute(string? viewName, Type? viewModelType = null, string? serviceName = null,
            ServiceLifetime lifetime = ServiceLifetime.Singleton) {
            ViewName = viewName;
            Lifetime = lifetime;
            ViewModelType = viewModelType;
            ServiceName = serviceName;
        }
    }
}