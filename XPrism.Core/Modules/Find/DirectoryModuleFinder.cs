using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace XPrism.Core.Modules.Find {
    /// <summary>
    /// 基于目录的模块发现器
    /// </summary>
    public class DirectoryModuleFinder : IModuleFinder {
        private readonly string _modulePath;
        private readonly string _modulePattern;

        public DirectoryModuleFinder(string modulePath = "Modules", string modulePattern = "*.dll") {
            _modulePath = modulePath;
            _modulePattern = modulePattern;
        }

        public IEnumerable<ModuleInfo> FindModules() {
            var moduleList = new List<ModuleInfo>();
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _modulePath);

            if (!Directory.Exists(path))
                return moduleList;
            var get = Directory.GetFiles(path, _modulePattern);
            foreach (var file in Directory.GetFiles(path, _modulePattern))
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
    }
}