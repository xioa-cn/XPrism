namespace XPrism.Core.DI;

public static class NavigationWindow {
    /// <summary>
    /// 获取窗口服务
    /// </summary>
    /// <param name="resourceKey">资源名（在容器内的名称）</param>
    /// <returns></returns>
    public static System.Windows.Window? Fetch(string resourceKey) {
        return XPrism.Core.DI.ContainerLocator.Container
            .GetService(resourceKey) as System.Windows.Window;
    }
}