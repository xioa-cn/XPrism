using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace XPrism.Core.Modules.Find;

public class DllModuleFinder : IModuleFinder {
    private readonly string _modulePath;
    private readonly List<string>? _modulePaths;

    /// <summary>
    /// Dll位置
    /// </summary>
    /// <param name="modulePath"></param>
    /// <param name="baseFile"></param>
    public DllModuleFinder(string modulePath, bool baseFile = true) {
        _modulePath = baseFile ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, modulePath + ".dll") : modulePath;
    }

    public DllModuleFinder(string file, params string[] modulePath) {
        _modulePaths = new List<string>();
        foreach (var item in modulePath)
        {
            _modulePaths.Add(Path.Combine(file, item + ".dll"));
        }
    }

    public IEnumerable<ModuleInfo> FindModules() {
        var moduleList = new List<ModuleInfo>();
        if (_modulePaths is not null)
        {
            foreach (var file in _modulePaths)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(file);
                    var moduleTypes = assembly.GetTypes()
                        .Where(t => typeof(IModule).IsAssignableFrom(t) && !t.IsAbstract);

                    foreach (var moduleType in moduleTypes)
                    {
                        var moduleAttr = moduleType.GetCustomAttribute<ModuleAttribute>();
                        if (moduleAttr == null) continue;

                        moduleList.Add(new ModuleInfo(
                            moduleType,
                            moduleAttr.ModuleName,
                            moduleAttr.IsOnDemand,
                            moduleAttr.DependsOn
                        ));
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to load module from {file}: {ex.Message}");
                }
            }

            return moduleList;
        }


        if (!File.Exists(_modulePath))
            return moduleList;

        try
        {
            var assembly = Assembly.LoadFrom(_modulePath);
            var moduleTypes = assembly.GetTypes()
                .Where(t => typeof(IModule).IsAssignableFrom(t) && !t.IsAbstract);

            foreach (var moduleType in moduleTypes)
            {
                var moduleAttr = moduleType.GetCustomAttribute<ModuleAttribute>();
                if (moduleAttr == null) continue;

                moduleList.Add(new ModuleInfo(
                    moduleType,
                    moduleAttr.ModuleName,
                    moduleAttr.IsOnDemand,
                    moduleAttr.DependsOn
                ));
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to load module from {_modulePath}: {ex.Message}");
        }

        return moduleList;
    }
}