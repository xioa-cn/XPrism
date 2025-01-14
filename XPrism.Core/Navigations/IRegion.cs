namespace XPrism.Core.Navigations;

/// <summary>
/// 区域接口
/// </summary>
public interface IRegion {
    /// <summary>
    /// 区域名称
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 当前视图
    /// </summary>
    object? CurrentView { get; }

    /// <summary>
    /// 注册的视图类型集合
    /// </summary>
    IEnumerable<Type> ViewTypes { get; }

    /// <summary>
    /// 导航到指定视图
    /// </summary>
    Task<bool> NavigateAsync(string viewName, INavigationParameters parameters, Type? vmType = null);
    
    /// <summary>
    /// 获取View的类型
    /// </summary>
    /// <param name="viewName"></param>
    /// <returns></returns>
    Type? GetViewType(string viewName);
    
    /// <summary>
    /// 重置页面
    /// </summary>
    /// <param name="viewName"></param>
    void ResetView(string viewName);
    /// <summary>
    /// 重置Vm
    /// </summary>
    /// <param name="vmTypeName"></param>
    void ResetVm(Type? vmTypeName);
    /// <summary>
    /// 重置页面和Vm
    /// </summary>
    /// <param name="viewName"></param>
    /// <param name="vmType"></param>
    void ResetViews(string viewName,Type vmType);
    
    

    /// <summary>
    /// 注册视图
    /// </summary>
    void RegisterView(string viewName, Type viewType);

   

    /// <summary>
    /// 移除视图
    /// </summary>
    void RemoveView(string viewName);
}