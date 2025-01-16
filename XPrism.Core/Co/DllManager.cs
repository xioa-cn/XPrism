using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.Loader;
using System.Windows.Media.Imaging;

namespace XPrism.Core.Co;


/// <summary>
/// 使用Assembly加载dll 尽可能的卸载dll
/// </summary>
public  class DllManager : IDllManager
{
    private  readonly Dictionary<string, (Assembly Assembly, WeakReference Instance)> _loadedAssemblies = new();
    private  bool _disposed;

    /// <summary>
    /// 加载DLL
    /// </summary>
    /// <param name="dllPath">DLL路径</param>
    /// <param name="name">可选的名称标识</param>
    /// <returns>加载的程序集</returns>
    public  Assembly LoadDll(string dllPath, string? name = null)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(DllManager));

        if (!File.Exists(dllPath))
            throw new FileNotFoundException("DLL file not found", dllPath);

        try
        {
            var key = name ?? Path.GetFileNameWithoutExtension(dllPath);
            key = key.Replace(".dll", "");

            // 检查是否已加载
            if (_loadedAssemblies.TryGetValue(key, out var existing))
            {
                if (existing.Instance.IsAlive)
                {
                    return existing.Assembly;
                }
                // 如果实例已被回收，移除记录
                _loadedAssemblies.Remove(key);
            }

            var assembly = Assembly.LoadFrom(dllPath);
            _loadedAssemblies[key] = (assembly, new WeakReference(assembly));

            Debug.WriteLine($"Successfully loaded assembly: {key}");
            return assembly;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to load assembly from {dllPath}: {ex}");
            throw;
        }
    }

    /// <summary>
    /// 卸载指定的DLL
    /// </summary>
    /// <param name="dllPath">DLL路径</param>
    public  void UnloadDll(string dllPath)
    {
        var key = Path.GetFileNameWithoutExtension(dllPath).Replace(".dll", "");
        if (_loadedAssemblies.Remove(key))
        {
            Debug.WriteLine($"Unloading assembly: {key}");
            CleanupResources();
        }
    }

    /// <summary>
    /// 卸载所有已加载的DLL
    /// </summary>
    public  void UnloadAll()
    {
        if (_loadedAssemblies.Count > 0)
        {
            Debug.WriteLine("Unloading all assemblies");
            _loadedAssemblies.Clear();
            CleanupResources();
        }
    }

    /// <summary>
    /// 清理资源
    /// </summary>
    private  void CleanupResources()
    {
        try
        {
            // 清理图片资源
            foreach (var (assembly, _) in _loadedAssemblies.Values)
            {
                foreach (var resourceName in assembly.GetManifestResourceNames())
                {
                    if (IsImageResource(resourceName))
                    {
                        try
                        {
                            var uri = new Uri($"pack://application:,,,/{assembly.GetName().Name};component/{resourceName}");
                            var bitmap = new BitmapImage(uri);
                            bitmap.StreamSource?.Dispose();
                            bitmap.UriSource = null;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Error cleaning up image resource {resourceName}: {ex.Message}");
                        }
                    }
                }
            }

            // 建议进行垃圾回收
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Debug.WriteLine("Resource cleanup completed");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error during cleanup: {ex.Message}");
        }
    }

    /// <summary>
    /// 检查资源是否为图片
    /// </summary>
    private static bool IsImageResource(string resourceName)
    {
        return resourceName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
               resourceName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
               resourceName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
               resourceName.EndsWith(".gif", StringComparison.OrdinalIgnoreCase) ||
               resourceName.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase) ||
               resourceName.EndsWith(".ico", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 检查指定的程序集是否已加载
    /// </summary>
    /// <param name="name">程序集名称</param>
    /// <returns>是否已加载</returns>
    public  bool IsLoaded(string name)
    {
        name = name.Replace(".dll", "");
        return _loadedAssemblies.TryGetValue(name, out var info) && 
               info.Instance.IsAlive;
    }

    /// <summary>
    /// 获取已加载的程序集
    /// </summary>
    /// <param name="name">程序集名称</param>
    /// <returns>程序集实例，如果未找到则返回null</returns>
    public  Assembly GetLoadedAssembly(string name)
    {
        name = name.Replace(".dll", "");
        return _loadedAssemblies.TryGetValue(name, out var info) && 
               info.Instance.IsAlive ? info.Assembly : null;
    }

    /// <summary>
    /// 获取所有已加载的程序集名称
    /// </summary>
    public  IEnumerable<string> GetLoadedAssemblyNames()
    {
        return _loadedAssemblies.Where(x => x.Value.Instance.IsAlive)
                               .Select(x => x.Key);
    }

    /// <summary>
    /// 获取程序集的状态信息
    /// </summary>
    public  IEnumerable<(string Name, bool IsAlive)> GetAssemblyStatus()
    {
        return _loadedAssemblies.Select(kvp => 
            (kvp.Key, kvp.Value.Instance.IsAlive));
    }

    /// <summary>
    /// 获取当前内存使用情况
    /// </summary>
    public static long GetTotalMemoryUsage()
    {
        GC.Collect();
        return GC.GetTotalMemory(true);
    }

    /// <summary>
    /// 获取程序集的资源名称列表
    /// </summary>
    public  IEnumerable<string> GetAssemblyResources(string name)
    {
        if (_loadedAssemblies.TryGetValue(name.Replace(".dll", ""), out var info) && 
            info.Instance.IsAlive)
        {
            return info.Assembly.GetManifestResourceNames();
        }
        return Enumerable.Empty<string>();
    }

    /// <summary>
    /// 释放所有资源
    /// </summary>
    public  void Dispose()
    {
        if (!_disposed)
        {
            UnloadAll();
            _disposed = true;
        }
    }
}


