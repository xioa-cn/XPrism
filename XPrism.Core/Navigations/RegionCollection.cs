using System.Collections;

namespace XPrism.Core.Navigations;

/// <summary>
/// 区域集合实现
/// </summary>
public class RegionCollection : IRegionCollection
{
    private readonly Dictionary<string, IRegion> _regions = new();

    /// <summary>
    /// 添加区域
    /// </summary>
    /// <param name="regionName">区域名称</param>
    /// <param name="region">区域实例</param>
    /// <exception cref="ArgumentException">当区域名称已存在时抛出</exception>
    public void Add(string regionName, IRegion region)
    {
        if (string.IsNullOrEmpty(regionName))
            throw new ArgumentNullException(nameof(regionName));
            
        if (region == null)
            throw new ArgumentNullException(nameof(region));

        if (_regions.ContainsKey(regionName))
            throw new ArgumentException($"Region with name '{regionName}' already exists.");

        _regions.Add(regionName, region);
    }

    /// <summary>
    /// 获取区域
    /// </summary>
    /// <param name="regionName">区域名称</param>
    /// <returns>区域实例</returns>
    /// <exception cref="KeyNotFoundException">当区域不存在时抛出</exception>
    public IRegion GetRegion(string regionName)
    {
        if (string.IsNullOrEmpty(regionName))
            throw new ArgumentNullException(nameof(regionName));

        if (!_regions.TryGetValue(regionName, out var region))
            throw new KeyNotFoundException($"Region '{regionName}' does not exist.");

        return region;
    }

    /// <summary>
    /// 移除区域
    /// </summary>
    /// <param name="regionName">区域名称</param>
    /// <returns>是否成功移除</returns>
    public bool Remove(string regionName)
    {
        if (string.IsNullOrEmpty(regionName))
            return false;

        return _regions.Remove(regionName);
    }

    /// <summary>
    /// 检查区域是否存在
    /// </summary>
    /// <param name="regionName">区域名称</param>
    /// <returns>是否存在</returns>
    public bool ContainsRegionWithName(string regionName)
    {
        if (string.IsNullOrEmpty(regionName))
            return false;

        return _regions.ContainsKey(regionName);
    }

    /// <summary>
    /// 获取区域集合的枚举器
    /// </summary>
    public IEnumerator<IRegion> GetEnumerator()
    {
        return _regions.Values.GetEnumerator();
    }

    /// <summary>
    /// 获取区域集合的枚举器
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// 清空所有区域
    /// </summary>
    public void Clear()
    {
        _regions.Clear();
    }

    /// <summary>
    /// 获取区域数量
    /// </summary>
    public int Count => _regions.Count;

    /// <summary>
    /// 获取所有区域名称
    /// </summary>
    public IEnumerable<string> Names => _regions.Keys;
}