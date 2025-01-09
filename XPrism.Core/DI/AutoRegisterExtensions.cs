using System.Reflection;
using XPrism.Core.DataContextWindow;
using XPrism.Core.Events;

namespace XPrism.Core.DI;

/// <summary>
/// 自动注册的扩展方法
/// </summary>
public static class AutoRegisterExtensions {
    /// <summary>
    /// 自动注册程序集中的类型
    /// </summary>
    /// <param name="containerRegistry">容器注册表</param>
    /// <param name="assemblies">要扫描的程序集</param>
    /// <param name="filter">类型过滤器</param>
    /// <returns>容器注册表实例</returns>
    public static IContainerRegistry AutoRegister(
        this IContainerRegistry containerRegistry,
        IEnumerable<Assembly> assemblies,
        Func<Type, bool>? filter = null) {
        var types = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsAbstract && !t.IsInterface && !t.IsGenericType);

        if (filter != null)
        {
            types = types.Where(filter);
        }

        foreach (var type in types)
        {
            // 获取类型实现的接口
            var interfaces = type.GetInterfaces()
                .Where(i => !i.IsGenericType && !IsSystemInterface(i));

            foreach (var @interface in interfaces)
            {
                containerRegistry.RegisterTransient(@interface, type);
            }
        }

        return containerRegistry;
    }

    /// <summary>
    /// 自动注册当前程序集中的类型
    /// </summary>
    /// <param name="containerRegistry">容器注册表</param>
    /// <param name="filter">类型过滤器</param>
    /// <returns>容器注册表实例</returns>
    public static IContainerRegistry AutoRegister(
        this IContainerRegistry containerRegistry,
        Func<Type, bool>? filter = null) {
        return containerRegistry.AutoRegister(new[] { Assembly.GetExecutingAssembly() }, filter);
    }


    /// <summary>
    /// 注册事件聚合器 
    /// </summary>
    /// <param name="containerRegistry"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IContainerRegistry RegisterEventAggregator<T>(this IContainerRegistry containerRegistry)
        where T : IEventAggregator {
        containerRegistry.RegisterSingleton<IEventAggregator, T>();
        return containerRegistry;
    }

    /// <summary>
    /// 自动注册指定命名空间下的类型
    /// </summary>
    /// <param name="containerRegistry">容器注册表</param>
    /// <param name="namespacePrefix">命名空间前缀</param>
    /// <returns>容器注册表实例</returns>
    public static IContainerRegistry AutoRegisterByNamespace(
        this IContainerRegistry containerRegistry,
        string namespacePrefix) {
        return containerRegistry.AutoRegister(
            new[] { Assembly.GetExecutingAssembly() },
            type => type.Namespace?.StartsWith(namespacePrefix) == true);
    }

    /// <summary>
    /// 自动注册带有特定特性的类型
    /// </summary>
    /// <typeparam name="TAttribute">特性类型</typeparam>
    /// <param name="containerRegistry">容器注册表</param>
    /// <returns>容器注册表实例</returns>
    public static IContainerRegistry AutoRegisterByAttribute<TAttribute>(
        this IContainerRegistry containerRegistry,
        params Assembly[] assemblies) where TAttribute : XPrismViewModelAttribute {
        var types = (assemblies.Length == 0
                ? new[] { Assembly.GetExecutingAssembly() }
                : assemblies)
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsAbstract && !t.IsInterface &&
                        t.GetCustomAttribute<TAttribute>() != null);

        foreach (var type in types)
        {
            RegisterTypeByWindowAttribute(containerRegistry, type);
        }

        return containerRegistry;
    }

    /// <summary>
    /// 根据特性注册类型
    /// </summary>
    private static void RegisterTypeByAttribute(
        IContainerRegistry containerRegistry,
        Type type) {
        var attribute = type.GetCustomAttribute<AutoRegisterAttribute>();
        if (attribute == null) return;

        // 如果指定了服务类型，直接注册
        if (attribute.ServiceType != null)
        {
            RegisterByLifetime(
                containerRegistry,
                attribute.ServiceType,
                type,
                attribute.Lifetime,
                attribute.ServiceName);
            return;
        }

        // 获取类型实现的接口
        var interfaces = type.GetInterfaces()
            .Where(i => !i.IsGenericType && !IsSystemInterface(i));

        // 如果没有实现接口，使用类型本身
        if (!interfaces.Any())
        {
            RegisterByLifetime(
                containerRegistry,
                type,
                type,
                attribute.Lifetime,
                attribute.ServiceName);
            return;
        }

        // 注册所有接口
        foreach (var @interface in interfaces)
        {
            RegisterByLifetime(
                containerRegistry,
                @interface,
                type,
                attribute.Lifetime,
                attribute.ServiceName);
        }
    }

    /// <summary>
    /// 根据特性注册类型
    /// </summary>
    private static void RegisterTypeByWindowAttribute(
        IContainerRegistry containerRegistry,
        Type type) {
        var attribute = type.GetCustomAttribute<XPrismViewModelAttribute>();
        if (attribute == null) return;

        RegisterByLifetime(
            containerRegistry,
            type,
            type,
            attribute.Lifetime,
            attribute.ViewName);
        return;
    }

    private static void RegisterByLifetime(
        IContainerRegistry containerRegistry,
        Type serviceType,
        Type implementationType,
        ServiceLifetime lifetime,
        string? serviceName = null) {
        if (serviceName != null)
        {
            switch (lifetime)
            {
                case ServiceLifetime.Singleton:
                    containerRegistry.RegisterSingleton(serviceType, implementationType, serviceName);
                    break;
                case ServiceLifetime.Scoped:
                    containerRegistry.RegisterScoped(serviceType, implementationType, serviceName);
                    break;
                case ServiceLifetime.Transient:
                    containerRegistry.RegisterTransient(serviceType, implementationType, serviceName);
                    break;
            }
        }
        else
        {
            switch (lifetime)
            {
                case ServiceLifetime.Singleton:
                    containerRegistry.RegisterSingleton(serviceType, implementationType);
                    break;
                case ServiceLifetime.Scoped:
                    containerRegistry.RegisterScoped(serviceType, implementationType);
                    break;
                case ServiceLifetime.Transient:
                    containerRegistry.RegisterTransient(serviceType, implementationType);
                    break;
            }
        }
    }

    /// <summary>
    /// 自动注册带有特定特性的类型
    /// </summary>
    public static IContainerRegistry AutoRegisterByAttribute(
        this IContainerRegistry containerRegistry,
        params Assembly[] assemblies) {
        var types = (assemblies.Length == 0
                ? new[] { Assembly.GetExecutingAssembly() }
                : assemblies)
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsAbstract && !t.IsInterface &&
                        t.GetCustomAttribute<AutoRegisterAttribute>() != null);

        foreach (var type in types)
        {
            RegisterTypeByAttribute(containerRegistry, type);
        }

        return containerRegistry;
    }

    private static bool IsSystemInterface(Type type) {
        return type.Namespace?.StartsWith("System") == true ||
               type.Namespace?.StartsWith("Microsoft") == true;
    }
}