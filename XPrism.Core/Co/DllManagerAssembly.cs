using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace XPrism.Core.Co;

/// <summary>
/// 使用Assembly 不卸载
/// </summary>
public class DllManagerAssembly : IDllManager {
    private readonly Dictionary<string, Assembly> _loadedAssemblies = new();
    private bool _disposed;

    public Assembly LoadDll(string dllPath, string? name = null) {
        if (_disposed)
            throw new ObjectDisposedException(nameof(DllManager));

        if (!File.Exists(dllPath))
            throw new FileNotFoundException("DLL file not found", dllPath);

        try
        {
            // 规范化key
            var key = name ?? Path.GetFileNameWithoutExtension(dllPath);
            key = key.Replace(".dll", "");

            // 检查是否已加载
            if (_loadedAssemblies.TryGetValue(key, out var loadedAssembly))
            {
                return loadedAssembly;
            }

            // 直接使用Assembly.LoadFrom加载程序集
            var assembly = Assembly.LoadFrom(dllPath);
            _loadedAssemblies[key] = assembly;

            return assembly;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to load assembly from {dllPath}: {ex}");
            throw;
        }
    }

    public void UnloadDll(string dllPath) {
        var key = Path.GetFileNameWithoutExtension(dllPath).Replace(".dll", "");
        _loadedAssemblies.Remove(key);
    }

    public void UnloadAll() {
        _loadedAssemblies.Clear();
    }

    public bool IsLoaded(string name) {
        name = name.Replace(".dll", "");
        return _loadedAssemblies.ContainsKey(name);
    }
}