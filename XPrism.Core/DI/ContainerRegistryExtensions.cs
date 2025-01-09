using XPrism.Core.Dialogs;
using XPrism.Core.Modules;
using XPrism.Core.Navigations;

namespace XPrism.Core.DI;

/// <summary>
/// IContainerRegistry的扩展方法
/// </summary>
public static class ContainerRegistryExtensions {
    /// <summary>
    /// 注册一个瞬时生命周期的类型映射
    /// </summary>
    /// <typeparam name="TFrom">服务类型</typeparam>
    /// <typeparam name="TTo">实现类型</typeparam>
    /// <returns>容器注册表实例</returns>
    public static IContainerRegistry RegisterTransient<TFrom, TTo>(this IContainerRegistry containerRegistry)
        where TTo : TFrom {
        return containerRegistry.RegisterTransient(typeof(TFrom), typeof(TTo));
    }

    /// <summary>
    /// 注册一个单例类型映射
    /// </summary>
    /// <typeparam name="TFrom">服务类型</typeparam>
    /// <typeparam name="TTo">实现类型</typeparam>
    /// <returns>容器注册表实例</returns>
    public static IContainerRegistry RegisterSingleton<TFrom, TTo>(this IContainerRegistry containerRegistry)
        where TTo : TFrom {
        return containerRegistry.RegisterSingleton(typeof(TFrom), typeof(TTo));
    }

    private static IContainerRegistry RegisterSingleton<TFrom, TTo>(this IContainerRegistry containerRegistry,
        Action<TFrom>? registerAction)
        where TTo : TFrom {
        return containerRegistry.RegisterSingleton(typeof(TFrom), typeof(TTo), registerAction);
    }

    // public static IContainerRegistry RegisterMeModuleManager(this IContainerRegistry containerRegistry) {
    //     return containerRegistry
    //         .RegisterSingleton<IModuleManager, ModuleManager>();
    // }

    /// <summary>
    /// 注册模块管理（在注册之前需要先注册一下 IModuleFinder（模块发现接口）实现）
    /// </summary>
    /// <param name="containerRegistry"></param>
    /// <param name="registerAction"></param>
    /// <returns></returns>
    public static IContainerRegistry RegisterMeModuleManager(this IContainerRegistry containerRegistry,
        Action<IModuleManager>? registerAction = null) {
        return containerRegistry
            .RegisterSingleton<IModuleManager, ModuleManager>(registerAction);
    }

    public static IContainerRegistry RegistryNavigations(this IContainerRegistry containerRegistry,
        Action<IRegionManager>? registerAction = null) {
        containerRegistry
            .RegisterSingleton<IRegionManager, RegionManager>();
        var regionManager = XPrismIoc.Fetch<IRegionManager>();
        RegionManagerProperty.SetRegionManager(regionManager);
        if (registerAction != null)
        {
            registerAction(regionManager);
        }

        return containerRegistry;
    }

    /// <summary>
    /// 注册一个单例实例
    /// </summary>
    /// <typeparam name="T">服务类型</typeparam>
    /// <param name="containerRegistry">容器注册表</param>
    /// <param name="instance">实例对象</param>
    /// <returns>容器注册表实例</returns>
    public static IContainerRegistry RegisterInstance<T>(this IContainerRegistry containerRegistry, T instance) {
        return containerRegistry.RegisterInstance(typeof(T), instance);
    }

    /// <summary>
    /// 注册一个作用域生命周期的类型映射
    /// </summary>
    /// <typeparam name="TFrom">服务类型</typeparam>
    /// <typeparam name="TTo">实现类型</typeparam>
    /// <returns>容器注册表实例</returns>
    public static IContainerRegistry RegisterScoped<TFrom, TTo>(this IContainerRegistry containerRegistry)
        where TTo : TFrom {
        return containerRegistry.RegisterScoped(typeof(TFrom), typeof(TTo));
    }

    /// <summary>
    /// 注册一个作用域生命周期的命名类型映射
    /// </summary>
    /// <typeparam name="TFrom">服务类型</typeparam>
    /// <typeparam name="TTo">实现类型</typeparam>
    /// <param name="containerRegistry">容器注册表</param>
    /// <param name="name">服务名称</param>
    /// <returns>容器注册表实例</returns>
    public static IContainerRegistry RegisterScoped<TFrom, TTo>(this IContainerRegistry containerRegistry, string name)
        where TTo : TFrom {
        return containerRegistry.RegisterScoped(typeof(TFrom), typeof(TTo), name);
    }

    /// <summary>
    /// 注册一个瞬时生命周期的命名类型映射
    /// </summary>
    /// <typeparam name="TFrom">服务类型</typeparam>
    /// <typeparam name="TTo">实现类型</typeparam>
    /// <param name="containerRegistry">容器注册表</param>
    /// <param name="name">服务名称</param>
    /// <returns>容器注册表实例</returns>
    public static IContainerRegistry RegisterTransient<TFrom, TTo>(
        this IContainerRegistry containerRegistry, string name) where TTo : TFrom {
        return containerRegistry.RegisterTransient(typeof(TFrom), typeof(TTo), name);
    }

    /// <summary>
    /// 注册一个单例类型映射
    /// </summary>
    /// <typeparam name="TFrom">服务类型</typeparam>
    /// <typeparam name="TTo">实现类型</typeparam>
    /// <param name="containerRegistry">容器注册表</param>
    /// <param name="name">服务名称</param>
    /// <returns>容器注册表实例</returns>
    public static IContainerRegistry RegisterSingleton<TFrom, TTo>(
        this IContainerRegistry containerRegistry, string name) where TTo : TFrom {
        return containerRegistry.RegisterSingleton(typeof(TFrom), typeof(TTo), name);
    }

    /// <summary>
    /// 解析指定类型的实例
    /// </summary>
    /// <typeparam name="T">要解析的类型</typeparam>
    /// <param name="containerRegistry">容器注册表</param>
    /// <returns>解析出的实例</returns>
    public static T Resolve<T>(this IContainerRegistry containerRegistry) {
        return (T)containerRegistry.Resolve(typeof(T));
    }

    /// <summary>
    /// 解析指定类型的命名实例
    /// </summary>
    /// <typeparam name="T">要解析的类型</typeparam>
    /// <param name="containerRegistry">容器注册表</param>
    /// <param name="name">服务名称</param>
    /// <returns>解析出的实例</returns>
    public static T ResolveNamed<T>(this IContainerRegistry containerRegistry, string name) {
        return (T)containerRegistry.ResolveNamed(typeof(T), name);
    }

    /// <summary>
    /// 创建一个新的作用域并执行指定的操作
    /// </summary>
    /// <param name="containerRegistry">容器注册表</param>
    /// <param name="action">要在作用域中执行的操作</param>
    public static void CreateScope(this IContainerRegistry containerRegistry, Action<IServiceScope> action) {
        using var scope = containerRegistry.CreateScope();
        action(scope);
    }

    /// <summary>
    /// 创建一个新的作用域并执行指定的异步操作
    /// </summary>
    /// <param name="containerRegistry">容器注册表</param>
    /// <param name="action">要在作用域中执行的异步操作</param>
    /// <returns>异步任务</returns>
    public static async Task CreateScopeAsync(
        this IContainerRegistry containerRegistry,
        Func<IServiceScope, Task> action) {
        using var scope = containerRegistry.CreateScope();
        await action(scope);
    }

    /// <summary>
    /// 注册一个瞬时生命周期的类型
    /// </summary>
    /// <typeparam name="T">要注册的类型</typeparam>
    /// <returns>容器注册表实例</returns>
    public static IContainerRegistry RegisterTransient<T>(this IContainerRegistry containerRegistry)
        where T : class {
        return containerRegistry.RegisterTransient(typeof(T), typeof(T));
    }


    /// <summary>
    /// 注册Dialog使用的基本类
    /// </summary>
    /// <param name="containerRegistry"></param>
    /// <returns></returns>
    public static IContainerRegistry RegisterDialogServiceCommonBase(this IContainerRegistry containerRegistry) {
        containerRegistry.RegisterSingleton<IDialogService, DialogService>();
        containerRegistry.RegisterSingleton<IDialogPresenter, DialogPresenter>();
        return containerRegistry;
    }

    /// <summary>
    /// 注册Dialog 需要先注册 RegisterDialogServiceCommonBase
    /// </summary>
    /// <param name="containerRegistry"></param>
    /// <typeparam name="T">Dialog类</typeparam>
    /// <typeparam name="TD">Dialog视图</typeparam>
    /// <returns></returns>
    public static IContainerRegistry RegisterDialogService<T, TD>(this IContainerRegistry containerRegistry)
        where TD : class {
        containerRegistry.RegisterTransient<TD>();
        DialogPresenterHelper.RegisterDialogType<T, TD>();
        return containerRegistry;
    }

    /// <summary>
    /// 注册一个瞬时生命周期的命名类型
    /// </summary>
    /// <typeparam name="T">要注册的类型</typeparam>
    /// <param name="containerRegistry">容器注册表</param>
    /// <param name="name">服务名称</param>
    /// <returns>容器注册表实例</returns>
    public static IContainerRegistry RegisterTransient<T>(
        this IContainerRegistry containerRegistry, string name) where T : class {
        return containerRegistry.RegisterTransient(typeof(T), typeof(T), name);
    }

    /// <summary>
    /// 注册一个单例生命周期的类型
    /// </summary>
    /// <typeparam name="T">要注册的类型</typeparam>
    /// <returns>容器注册表实例</returns>
    public static IContainerRegistry RegisterSingleton<T>(this IContainerRegistry containerRegistry)
        where T : class {
        return containerRegistry.RegisterSingleton(typeof(T), typeof(T));
    }

    /// <summary>
    /// 注册一个单例生命周期的命名类型
    /// </summary>
    /// <typeparam name="T">要注册的类型</typeparam>
    /// <param name="containerRegistry">容器注册表</param>
    /// <param name="name">服务名称</param>
    /// <returns>容器注册表实例</returns>
    public static IContainerRegistry RegisterSingleton<T>(
        this IContainerRegistry containerRegistry, string name) where T : class {
        return containerRegistry.RegisterSingleton(typeof(T), typeof(T), name);
    }

    /// <summary>
    /// 注册一个作用域生命周期的类型
    /// </summary>
    /// <typeparam name="T">要注册的类型</typeparam>
    /// <returns>容器注册表实例</returns>
    public static IContainerRegistry RegisterScoped<T>(this IContainerRegistry containerRegistry)
        where T : class {
        return containerRegistry.RegisterScoped(typeof(T), typeof(T));
    }

    /// <summary>
    /// 注册一个作用域生命周期的命名类型
    /// </summary>
    /// <typeparam name="T">要注册的类型</typeparam>
    /// <param name="containerRegistry">容器注册表</param>
    /// <param name="name">服务名称</param>
    /// <returns>容器注册表实例</returns>
    public static IContainerRegistry RegisterScoped<T>(
        this IContainerRegistry containerRegistry, string name) where T : class {
        return containerRegistry.RegisterScoped(typeof(T), typeof(T), name);
    }
}