using XPrism.Core.Modules;

namespace XPrism.Core.DI {
    /// <summary>
    /// 定义容器注册表的接口，用于配置依赖注入容器
    /// </summary>
    public interface IContainerRegistry {
        public IContainerExtension<IServiceProvider> GetIContainerExtension();

        //internal  IContainerExtension<IServiceProvider> _container { get; }
        /// <summary>
        /// 注册一个瞬时生命周期的类型映射
        /// </summary>
        /// <param name="from">服务类型</param>
        /// <param name="to">实现类型</param>
        /// <returns>容器注册表实例</returns>
        IContainerRegistry RegisterTransient(Type from, Type to);

        //IContainerRegistry RegisterModules(string[] assemblies);

        /// <summary>
        /// 注册一个瞬时生命周期的命名类型映射
        /// </summary>
        /// <param name="from">服务类型</param>
        /// <param name="to">实现类型</param>
        /// <param name="name">服务名称</param>
        /// <returns>容器注册表实例</returns>
        IContainerRegistry RegisterTransient(Type from, Type to, string name);

        /// <summary>
        /// 注册一个单例实例
        /// </summary>
        /// <param name="type">服务类型</param>
        /// <param name="instance">实例对象</param>
        /// <returns>容器注册表实例</returns>
        IContainerRegistry RegisterInstance(Type type, object instance);

        /// <summary>
        /// 注册一个单例类型映射
        /// </summary>
        /// <param name="from">服务类型</param>
        /// <param name="to">实现类型</param>
        /// <returns>容器注册表实例</returns>
        IContainerRegistry RegisterSingleton(Type from, Type to);

        IContainerRegistry RegisterSingleton<T>(Type from, Type to, Action<T>? registerAction);


        IContainerRegistry Initialized();

        /// <summary>
        /// 注册一个单例类型映射
        /// </summary>
        /// <param name="from">服务类型</param>
        /// <param name="to">实现类型</param>
        /// <param name="name">服务名称</param>
        /// <returns>容器注册表实例</returns>
        public IContainerRegistry RegisterSingleton(Type from, Type to, string name);

        public IContainerRegistry RegisterSingleton<T>(
            T value, string name = null) where T : class;

        public IContainerRegistry RegisterScoped<T>(
            T value, string name = null) where T : class;

        /// <summary>
        /// 获取容器扩展实例
        /// </summary>
        /// <returns>容器扩展实例</returns>
        IContainerExtension GetContainer();

        /// <summary>
        /// 注册一个作用域生命周期的类型映射
        /// </summary>
        /// <param name="from">服务类型</param>
        /// <param name="to">实现类型</param>
        /// <returns>容器注册表实例</returns>
        IContainerRegistry RegisterScoped(Type from, Type to);

        /// <summary>
        /// 注册一个作用域生命周期的命名类型映射
        /// </summary>
        /// <param name="from">服务类型</param>
        /// <param name="to">实现类型</param>
        /// <param name="name">服务名称</param>
        /// <returns>容器注册表实例</returns>
        IContainerRegistry RegisterScoped(Type from, Type to, string name);

        /// <summary>
        /// 创建一个新的作用域
        /// </summary>
        /// <returns>作用域实例</returns>
        IServiceScope CreateScope();

        /// <summary>
        /// 解析一个类型的实例
        /// </summary>
        /// <param name="type">要解析的类型</param>
        /// <returns>解析出的实例</returns>
        object Resolve(Type type);

        /// <summary>
        /// 解析一个类型的实例
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns>解析出的实例</returns>
        object? Resolve(string serviceName);

        /// <summary>
        /// 解析一个命名类型的实例
        /// </summary>
        /// <param name="type">要解析的类型</param>
        /// <param name="name">服务名称</param>
        /// <returns>解析出的实例</returns>
        object ResolveNamed(Type type, string name);

        /// <summary>
        /// 构建容器并完成初始化
        /// </summary>
        /// <returns>容器实例</returns>
        IServiceProvider Build();

        /// <summary>
        /// 获取已构建的容器实例
        /// 如果容器未构建，则先构建容器
        /// </summary>
        IServiceProvider Container { get; }

        /// <summary>
        /// 通过服务名称获取服务实例
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <param name="serviceName">服务名称</param>
        /// <returns>服务实例</returns>
        T GetService<T>(string serviceName);

        /// <summary>
        /// 获取服务实例
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <returns>服务实例</returns>
        T GetService<T>();

        /// <summary>
        /// 通过名称获取实例
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        object? GetService(string serviceName);

        /// <summary>
        /// 重置资源实例
        /// </summary>
        /// <param name="serviceName"></param>
        void ResetService(string serviceName);
        
        /// <summary>
        /// 重置资源实例
        /// </summary>
        /// <param name="type"></param>
        void ResetService(Type type);

        /// <summary>
        /// 通过服务名称获取服务实例
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceName">服务名称</param>
        /// <returns>服务实例</returns>
        object GetService(Type serviceType, string serviceName);

        /// <summary>
        /// 获取服务实例
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <returns>服务实例</returns>
        object GetService(Type serviceType);
    }
}