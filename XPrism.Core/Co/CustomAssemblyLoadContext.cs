using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace XPrism.Core.Co;

public class CustomAssemblyLoadContext : AssemblyLoadContext
{
    private readonly Dictionary<string, Assembly> _loadedAssemblies = new();
    private readonly string _basePath;

    public CustomAssemblyLoadContext() : base(isCollectible: true)
    {
        _basePath = AppDomain.CurrentDomain.BaseDirectory;
        LoadBaseAssemblies();
    }

    private void LoadBaseAssemblies()
    {
        try
        {
            // 预加载基础程序集
            var baseAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => IsImportantAssembly(a.GetName().Name));

            foreach (var assembly in baseAssemblies)
            {
                if (!_loadedAssemblies.ContainsKey(assembly.GetName().Name))
                {
                    _loadedAssemblies[assembly.GetName().Name] = assembly;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading base assemblies: {ex.Message}");
        }
    }

    private bool IsImportantAssembly(string name)
    {
        return name.StartsWith("PresentationFramework") ||
               name.StartsWith("PresentationCore") ||
               name.StartsWith("WindowsBase") ||
               name.StartsWith("System.Xaml") ||
               name.StartsWith("XPrism");
    }

    protected override Assembly Load(AssemblyName assemblyName)
    {
        try
        {
            // 1. 检查已加载的程序集
            if (_loadedAssemblies.TryGetValue(assemblyName.Name, out var loadedAssembly))
            {
                return loadedAssembly;
            }

            // 2. 尝试从默认上下文加载
            var defaultAssembly = Default.LoadFromAssemblyName(assemblyName);
            if (defaultAssembly != null)
            {
                _loadedAssemblies[assemblyName.Name] = defaultAssembly;
                return defaultAssembly;
            }

            // 3. 尝试从基础目录加载
            var assemblyPath = Path.Combine(_basePath, $"{assemblyName.Name}.dll");
            if (File.Exists(assemblyPath))
            {
                var assembly = LoadFromAssemblyPath(assemblyPath);
                _loadedAssemblies[assemblyName.Name] = assembly;
                return assembly;
            }

            return null;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading assembly {assemblyName.Name}: {ex.Message}");
            return null;
        }
    }
}