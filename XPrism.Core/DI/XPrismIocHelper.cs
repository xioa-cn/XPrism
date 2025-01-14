using System.Security.Principal;
using XPrism.Core.DataContextWindow;

namespace XPrism.Core.DI;

public static class XPrismIoc {
    /// <summary>
    /// 获取容器内的资源
    /// </summary>
    /// <param name="resourceKey"></param>
    /// <returns></returns>
    public static object? Fetch(string resourceKey) {
        return XPrism.Core.DI.ContainerLocator.Container
            .GetService(resourceKey);
    }

    /// <summary>
    /// 获取容器内的资源
    /// </summary>
    /// <returns></returns>
    public static T? Fetch<T>() {
        return XPrism.Core.DI.ContainerLocator.Container
            .GetService<T>();
    }
    
    /// <summary>
    /// 获取容器内的资源
    /// </summary>
    /// <returns></returns>
    public static object? Fetch(Type type) {
        return XPrism.Core.DI.ContainerLocator.Container
            .GetService(type);
    }

    /// <summary>
    /// 获取容器内的XPrismWindow资源
    /// </summary>
    /// <param name="resourceKey"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static XPrismWindow FetchXPrismWindow(string resourceKey) {
        var window = XPrism.Core.DI.ContainerLocator.Container
            .GetService(resourceKey) as XPrismWindow;
        if (window is null)
            throw new Exception($"Can't find XPrism Window {resourceKey}");
        if (window.IsReallyClosing)
        {
            throw new Exception($"Can't create XPrism Window {resourceKey},window is really closing");
        }

        return window;
    }

    /// <summary>
    /// 注销XPrismWindow资源 注销后的window不可再次获取使用
    /// </summary>
    /// <param name="window"></param>
    public static void DisposeXPrismWindow(this XPrismWindow window) {
        window.Dispose();
    }

    /// <summary>
    /// 注销XPrismWindow资源 且再重新注册出一个Winodw资源让用户可以重新使用
    /// 只会消除你在 Window.xaml和 Window.xaml.cs中的资源 而不是ViewModel
    /// 会把window在容器里删除 再重新添加
    /// 注意：容器的资源模式是在用户首次使用时才创建对象 除非你是用实例注册的而不是类型 且在注册时没有使用初始化任务
    /// </summary>
    /// <param name="window"></param>
    public static void ResetXPrismWindow(this XPrismWindow window) {
        Type windowType = window.GetType();
        window.Dispose();
        var vm = windowType.GetCustomAttributes(typeof(XPrismViewModelAttribute), true)
            .FirstOrDefault() as XPrismViewModelAttribute;
        if (vm == null)
            return;
        if (vm.Lifetime == ServiceLifetime.Singleton)
        {
            ContainerLocator.Container.RegisterSingleton(windowType, windowType, vm.ViewName);
        }
        else if (vm.Lifetime == ServiceLifetime.Scoped)
        {
            ContainerLocator.Container.RegisterScoped(windowType, vm.ViewName);
        }
        else if (vm.Lifetime == ServiceLifetime.Transient)
        {
            ContainerLocator.Container.RegisterTransient(windowType, windowType);
        }
    }

    /// <summary>
    /// 重新注册资源，只有使用类型注册的可以重新注册， 实例注册的无法Reset
    /// </summary>
    /// <param name="resourceKey"></param>
    public static void ResetXPrismModel(string resourceKey) {
        XPrism.Core.DI.ContainerLocator.Container
            .ResetService(resourceKey);
    }
    /// <summary>
    /// 重新注册资源，只有使用类型注册的可以重新注册， 实例注册的无法Reset
    /// </summary>
    /// <param name="type"></param>
    public static void ResetXPrismModel(Type type) {
        XPrism.Core.DI.ContainerLocator.Container
            .ResetService(type);
    }

    /// <summary>
    /// 重新注册Window和他的Viewmodel
    /// </summary>
    /// <param name="window"></param>
    public static void ResetXPrismWindowVm(this XPrismWindow window) {
        Type windowType = window.GetType();
        window.Dispose();
        var vm = windowType.GetCustomAttributes(typeof(XPrismViewModelAttribute), true)
            .FirstOrDefault() as XPrismViewModelAttribute;
        if (vm == null)
            return;

        if (vm.ServiceName is not null)
            ResetXPrismModel(vm.ServiceName);

        if (vm.Lifetime == ServiceLifetime.Singleton)
        {
            ContainerLocator.Container.RegisterSingleton(windowType, windowType, vm.ViewName);
        }
        else if (vm.Lifetime == ServiceLifetime.Scoped)
        {
            ContainerLocator.Container.RegisterScoped(windowType, vm.ViewName);
        }
        else if (vm.Lifetime == ServiceLifetime.Transient)
        {
            ContainerLocator.Container.RegisterTransient(windowType, windowType);
        }
    }
}