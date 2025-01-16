using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace XPrism.Core.Co;

public class DllManagerAssemblyLoadContext : IDllManager {
    private readonly Dictionary<string, (CustomAssemblyLoadContext Context, System.Reflection.Assembly Assembly)>
        _loadedAssemblies = new();

    private bool _disposed;

    /// <summary>
    /// 加载DLL
    /// </summary>
    public Assembly LoadDll(string dllPath, string? name = null) {
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
                // 刷新上下文
                var assembly = existing.Context.LoadFromAssemblyPath(dllPath);
                return assembly;
            }

            // 创建新的上下文
            var context = new CustomAssemblyLoadContext();
            var assembly1 = context.LoadFromAssemblyPath(dllPath);

            _loadedAssemblies[key] = (context, assembly1);
            Debug.WriteLine($"Successfully loaded assembly: {key}");

            return assembly1;
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
    public void UnloadDll(string dllPath) {
        var key = Path.GetFileNameWithoutExtension(dllPath).Replace(".dll", "");
        if (_loadedAssemblies.TryGetValue(key, out var existing))
        {
            try
            {
                existing.Context.Unload();
                _loadedAssemblies.Remove(key);

                // 强制GC
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                Debug.WriteLine($"Successfully unloaded assembly: {key}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error unloading assembly {key}: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// 卸载所有已加载的DLL
    /// </summary>
    public void UnloadAll() {
        foreach (var (key, (context, _)) in _loadedAssemblies.ToList())
        {
            try
            {
                context.Unload();
                Debug.WriteLine($"Unloaded assembly: {key}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error unloading assembly {key}: {ex.Message}");
            }
        }

        _loadedAssemblies.Clear();

        // 强制GC
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }

    /// <summary>
    /// 检查指定的程序集是否已加载
    /// </summary>
    public bool IsLoaded(string name) {
        name = name.Replace(".dll", "");
        return _loadedAssemblies.ContainsKey(name);
    }

    /// <summary>
    /// 获取已加载的程序集
    /// </summary>
    public Assembly GetLoadedAssembly(string name) {
        name = name.Replace(".dll", "");
        return _loadedAssemblies.TryGetValue(name, out var info) ? info.Assembly : null;
    }

    /// <summary>
    /// 获取所有已加载的程序集名称
    /// </summary>
    public IEnumerable<string> GetLoadedAssemblyNames() {
        return _loadedAssemblies.Keys;
    }

    /// <summary>
    /// 获取程序集的状态信息
    /// </summary>
    public IEnumerable<(string Name, bool IsAlive)> GetAssemblyStatus() {
        return _loadedAssemblies.Select(kvp =>
            (kvp.Key, IsContextAlive(kvp.Value.Context)));
    }

    private bool IsContextAlive(CustomAssemblyLoadContext context) {
        try
        {
            // 尝试访问上下文，如果已卸载会抛出异常
            var _ = context.Name;
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 获取当前内存使用情况
    /// </summary>
    public long GetTotalMemoryUsage() {
        GC.Collect();
        return GC.GetTotalMemory(true);
    }

    /// <summary>
    /// 释放所有资源
    /// </summary>
    public void Dispose() {
        if (!_disposed)
        {
            UnloadAll();
            _disposed = true;
        }
    }
}