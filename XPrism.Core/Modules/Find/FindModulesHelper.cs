using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Shapes;
using System.Xml.Serialization;
using XPrism.Core.Modules.Config;
using Path = System.IO.Path;

namespace XPrism.Core.Modules.Find;

public static class FindModulesHelper {
    public static IModuleManager LoadModules(this IModuleManager manager, string moduleFileName, bool baseDir = true) {
        var moduleList = new List<ModuleInfo>();
        try
        {
            if (baseDir)
            {
                moduleFileName =
                    System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, moduleFileName + ".dll");
            }

            if (!System.IO.File.Exists(moduleFileName))
            {
                throw new FileNotFoundException();
            }

            var assembly = Assembly.LoadFrom(moduleFileName);
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
            Debug.WriteLine($"Failed to load module from {moduleFileName}: {ex.Message}");
        }

        manager.LoadModule(moduleList);
        return manager;
    }

    private static ModulesConfiguration? LoadConfiguration(string configFile) {
        var serializer = new XmlSerializer(typeof(ModulesConfiguration));

        using var reader = new StreamReader(configFile);
        if (reader == null)
            throw new FileNotFoundException();
        var result = serializer?.Deserialize(reader) as ModulesConfiguration;

        return result;
    }

    public static IModuleManager LoadModulesConfig(this IModuleManager manager, string? moduleFileName) {
        ArgumentNullException.ThrowIfNull(moduleFileName);

        moduleFileName = Path.Combine(moduleFileName, "modules.config");

        var modules = LoadConfiguration(moduleFileName);
        if(modules == null)
            throw new FileNotFoundException();
        if (modules.Name != "XPrismModules.Config")
        {
            throw new ConfigurationErrorsException();
        }

        foreach (var itemModule in modules.Modules.Modules)
        {
            foreach (var moduleName in itemModule.ModuleNames)
            {
                manager.LoadModule(itemModule.AssemblyFile,moduleName);
            }
        }
        
        return manager;
    }
}