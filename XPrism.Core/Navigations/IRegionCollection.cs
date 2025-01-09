namespace XPrism.Core.Navigations;

/// <summary>
/// 区域集合接口
/// </summary>
public interface IRegionCollection : IEnumerable<IRegion>
{
    /// <summary>
    /// 添加区域
    /// </summary>
    void Add(string regionName, IRegion region);

    /// <summary>
    /// 获取区域
    /// </summary>
    IRegion GetRegion(string regionName);

    /// <summary>
    /// 移除区域
    /// </summary>
    bool Remove(string regionName);

    /// <summary>
    /// 是否包含区域
    /// </summary>
    bool ContainsRegionWithName(string regionName);
} 