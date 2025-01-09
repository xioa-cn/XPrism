using System.Diagnostics;
using XPrism.Core.DebugLog;
using XPrism.Core.DI;

namespace XPrism.Core.Navigations;

/// <summary>
/// 导航服务实现
/// </summary>
public class NavigationService : INavigationService {
    private readonly IRegionManager _regionManager;
    private readonly Stack<NavigationContext> _navigationStack;
    private readonly IContainerProvider _container;

    public bool CanGoBack => _navigationStack.Count > 1;
    public object? CurrentView => _navigationStack.Count > 0 ? _navigationStack.Peek().Target : null;

    public NavigationService(
        IRegionManager regionManager
    ) {
        _regionManager = regionManager;
        _container = ContainerLocator.Container.GetIContainerExtension();
        _navigationStack = new Stack<NavigationContext>();
    }

    public async Task<bool> NavigateAsync(string path, INavigationParameters? parameters = null) {
        try
        {
            // 解析路径
            var (regionName, viewName) = ParsePath(path);
            if (string.IsNullOrEmpty(regionName) || string.IsNullOrEmpty(viewName))
            {
                throw new Exception("path is regionName and viewName => mainRegion/home ");
            }

            // 获取区域
            var region = _regionManager.Regions.GetRegion(regionName);

            // 创建导航上下文
            var context = new NavigationContext(
                path,
                parameters ?? new NavigationParameters(),
                CurrentView);

            // 处理当前视图的导航
            if (CurrentView is INavigationAware currentAware)
            {
                // 检查是否可以离开当前页面
                if (!await currentAware.CanNavigateFromAsync(context.Parameters))
                {
                    return false;
                }

                // 触发离开事件
                await currentAware.OnNavigatingFromAsync(context.Parameters);
            }

            var vmType =
                _regionManager.GetDataContext(viewName);

            // 执行导航
            var success = await region.NavigateAsync(viewName, context.Parameters, vmType);
            if (success)
            {
                // 创建新的导航上下文，包含目标视图
                var newContext = new NavigationContext(
                    path,
                    context.Parameters,
                    context.Source,
                    region.CurrentView);

                _navigationStack.Push(newContext);

                // 触发导航到事件
                if (region.CurrentView is INavigationAware targetAware)
                {
                    await targetAware.OnNavigatingToAsync(context.Parameters);
                }
            }

            return success;
        }
        catch (Exception ex)
        {
            // 可以添加日志记录
            DebugLogger.LogError($"Navigation failed: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> GoBackAsync() {
        if (!CanGoBack)
            return false;

        // 移除当前页面的上下文
        _navigationStack.Pop();

        // 获取上一个页面的上下文
        var previousContext = _navigationStack.Peek();

        // 导航到上一个页面
        return await NavigateAsync(
            previousContext.NavigationPath,
            previousContext.Parameters);
    }

    private (string RegionName, string ViewName) ParsePath(string path) {
        var parts = path.Split('/');
        if (parts.Length != 2)
        {
            return (string.Empty, string.Empty);
        }

        return (parts[0], parts[1]);
    }
}