using System;

namespace XPrism.Core.DI {
    /// <summary>
    /// 定义依赖注入容器的基本操作接口
    /// </summary>
    public interface IContainerExtension : IContainerProvider {
        /// <summary>
        /// 注册一个类型映射（瞬时生命周期）
        /// </summary>
        /// <param name="from">服务类型</param>
        /// <param name="to">实现类型</param>
        void RegisterTransient(Type from, Type to);

        /// <summary>
        /// 注册一个命名类型映射（瞬时生命周期）
        /// </summary>
        /// <param name="from">服务类型</param>
        /// <param name="to">实现类型</param>
        /// <param name="name">服务名称</param>
        void RegisterTransient(Type from, Type to, string name);

        /// <summary>
        /// 注册一个单例实例
        /// </summary>
        /// <param name="type">服务类型</param>
        /// <param name="instance">实例对象</param>
        void RegisterInstance(Type type, object instance);

        /// <summary>
        /// 注册一个单例实例
        /// </summary>
        /// <param name="type">服务类型</param>
        /// <param name="instance">实例对象</param>
        /// <param name="name">实例对象</param>
        void RegisterInstance(Type type, object instance, string? name);

        /// <summary>
        /// 注册一个单例类型映射（带名称）
        /// </summary>
        /// <param name="from">服务类型</param>
        /// <param name="to">实现类型</param>
        void RegisterSingleton(Type from, Type to);

        void RegisterSingleton<T>(Type from, Type to, Action<T>? registerAction);

        /// <summary>
        /// 注册一个单例类型映射（带名称）
        /// </summary>
        /// <param name="from">服务类型</param>
        /// <param name="to">实现类型</param>
        /// <param name="name">服务名称</param>
        void RegisterSingleton(Type from, Type to, string name);

        /// <summary>
        /// 解析一个类型的实例
        /// </summary>
        /// <param name="type">要解析的类型</param>
        /// <returns>解析出的实例</returns>
        object? Resolve(Type type);

        /// <summary>
        /// 解析一个类型的实例
        /// </summary>
        /// <param name="name"></param>
        /// <returns>解析出的实例</returns>
        object? Resolve(string name);

        /// <summary>
        /// 重新实例资源资源
        /// </summary>
        /// <param name="name"></param>
        void ResetService(string name);
        
        /// <summary>
        /// 重新实例资源资源
        /// </summary>
        /// <param name="type"></param>
        void ResetService(Type type);
        
        
        /// <summary>
        /// 解析一个命名类型的实例
        /// </summary>
        /// <param name="type">要解析的类型</param>
        /// <param name="name">服务名称</param>
        /// <returns>解析出的实例</returns>
        object ResolveNamed(Type type, string name);

        /// <summary>
        /// 完成容器的初始化配置
        /// </summary>
        void FinalizeExtension();

        /// <summary>
        /// 注册一个作用域生命周期的类型映射
        /// </summary>
        /// <param name="from">服务类型</param>
        /// <param name="to">实现类型</param>
        void RegisterScoped(Type from, Type to);

        /// <summary>
        /// 注册一个作用域生命周期的类型映射
        /// </summary>
        /// <param name="from">服务类型</param>
        /// <param name="to">实现类型</param>
        /// <param name="name">服务名称</param>
        void RegisterScoped(Type from, Type to, string name);

        /// <summary>
        /// 创建一个新的作用域
        /// </summary>
        /// <returns>作用域实例</returns>
        IServiceScope CreateScope();
    }

    /// <summary>
    /// 泛型容器扩展接口，提供对具体容器实例的访问
    /// </summary>
    /// <typeparam name="TContainer">容器类型</typeparam>
    public interface IContainerExtension<TContainer> : IContainerExtension {
        /// <summary>
        /// 获取实际的容器实例
        /// </summary>
        TContainer Instance { get; }
    }
}