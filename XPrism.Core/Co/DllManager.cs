using System.IO;
using System.Reflection;

namespace XPrism.Core.Co;

public static class DllManager {
    private static Dictionary<string, CustomAssemblyLoadContext> _loadedContexts = new();
    private static bool _disposed;

    public static Dictionary<string, CustomAssemblyLoadContext> LoadedContexts => _loadedContexts;

    public static Assembly LoadDll(string dllPath, string? name = null) {
        if (_disposed)
            throw new ObjectDisposedException(nameof(DllManager));

        if (!File.Exists(dllPath))
            throw new FileNotFoundException("DLL file not found", dllPath);

        var loadContext = new CustomAssemblyLoadContext();
        var assembly = loadContext.LoadFromAssemblyPath(dllPath);

        if (name == null)
        {
            name = dllPath;
        }
        else
        {
            name = name.Replace(".dll", "");
        }

        _loadedContexts[name] = loadContext;

        return assembly;
    }

    public static void UnloadDll(string dllPath) {
        if (_loadedContexts.TryGetValue(dllPath, out var context))
        {
            context.Unload();
            _loadedContexts.Remove(dllPath);
        }
    }

    public static void UnloadAll() {
        foreach (var context in _loadedContexts.Values)
        {
            context.Unload();
        }

        _loadedContexts.Clear();
    }
}