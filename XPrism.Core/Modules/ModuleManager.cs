using System.Diagnostics;
using System.IO;
using System.Reflection;
using XPrism.Core.DI;
using XPrism.Core.Modules.Find;

namespace XPrism.Core.Modules {
    /// <summary>
    /// 模块管理器实现
    /// </summary>
    public class ModuleManager : IModuleManager {
        private readonly IModuleFinder _moduleFinder;
        private readonly IContainerProvider _containerProvider;
        private readonly IContainerRegistry _containerRegistry;
        private readonly Dictionary<string, ModuleInfo> _modulesByName = new();
        private readonly HashSet<string> _initializedModules = new();

        public ModuleManager(
            IModuleFinder moduleFinder
        ) {
            _moduleFinder = moduleFinder;
            _containerProvider = ContainerLocator.Container.GetIContainerExtension();
            _containerRegistry = ContainerLocator.Container;
        }

        public void LoadModules() {
            // 发现所有模块
            LoadModule(_moduleFinder.FindModules());
        }

        public void LoadModule(IEnumerable<ModuleInfo> modules) {
            foreach (var module in modules)
            {
                _modulesByName[module.ModuleName] = module;
            }

            // 加载非按需加载的模块
            foreach (var module in modules.Where(m => !m.IsOnDemand))
            {
                LoadModule(module.ModuleName);
            }
        }


        public void LoadModule(string moduleName) {
            if (_initializedModules.Contains(moduleName))
                return;

            if (!_modulesByName.TryGetValue(moduleName, out var moduleInfo))
                throw new InvalidOperationException($"Module {moduleName} not found");

            // 先加载依赖模块
            foreach (var dependency in moduleInfo.DependsOn)
            {
                LoadModule(dependency);
            }

            // 创建模块实例
            var module = CreateModule(moduleInfo.ModuleType);

            try
            {
                // 注册模块服务
                module.RegisterTypes(_containerRegistry);

                // 初始化模块
                module.OnInitialized(_containerProvider);

                _initializedModules.Add(moduleName);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Failed to initialize module {moduleName}", ex);
            }
        }

        public void LoadModule(string assemblyPath, string moduleName) {
            var assemblyFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyPath);
            if (!assemblyFile.EndsWith(".dll"))
            {
                assemblyFile += ".dll";
            }

            var assembly = Assembly.LoadFrom(assemblyFile);
            var moduleTypes = assembly.GetTypes()
                .Where(t => typeof(IModule).IsAssignableFrom(t) && !t.IsAbstract);

            foreach (var moduleType in moduleTypes)
            {
                var moduleAttr = moduleType.GetCustomAttribute<ModuleAttribute>();
                if (moduleAttr == null) continue;

                if (moduleAttr.ModuleName != moduleName) continue;

                var info = new ModuleInfo(
                    moduleType,
                    moduleAttr.ModuleName,
                    moduleAttr.IsOnDemand,
                    moduleAttr.DependsOn
                );
                _modulesByName[info.ModuleName] = info;
                // 加载非按需加载的模块
                LoadModule(info.ModuleName);
                return;
            }

            throw new InvalidOperationException($"Module {moduleName} not found");
        }

        private IModule CreateModule(Type moduleType) {
            try
            {
                return (IModule)ActivatorUtilities.CreateInstance(_containerProvider, moduleType);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Failed to create module instance of type {moduleType.Name}", ex);
            }
        }
    }
}