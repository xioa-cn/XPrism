using System.Diagnostics;
using System.IO;
using System.Reflection;
using XPrism.Core.Co;
using XPrism.Core.DI;
using XPrism.Core.Modules.Find;

namespace XPrism.Core.Modules {
    /// <summary>
    /// 模块管理器实现
    /// </summary>
    public partial class ModuleManager : IModuleManager {
        private readonly IModuleFinder _moduleFinder;
        private readonly IContainerProvider _containerProvider;
        private readonly IContainerRegistry _containerRegistry;
        private readonly Dictionary<string, ModuleInfo> _modulesByName = new();
        private readonly HashSet<string> _initializedModules = new();
        private readonly Dictionary<string, ModuleInfo> _loadedModules = new();
        private readonly Dictionary<string, Assembly> _loadedAssemblies = new();

        public void RecordLoadedModule(string moduleName, ModuleInfo moduleInfo) {
            _loadedModules[moduleName] = moduleInfo;
        }


        public void RecordLoadedAssembly(string assemblyName, Assembly assembly) {
            _loadedAssemblies[assemblyName] = assembly;
            //DllManager.LoadDll(Path.Combine(baseDir, assemblyName));
        }

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
                // 记录模块实例
                moduleInfo.Instance = module;
                // 注册模块服务
                module.RegisterTypes(_containerRegistry);

                // 初始化模块
                module.OnInitialized(_containerProvider);

                RecordLoadedModule(moduleName, moduleInfo);
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

            Assembly assembly;
            if (_loadedAssemblies.TryGetValue(assemblyFile, out var loadedAssembly))
            {
                assembly = loadedAssembly;
            }
            else
            {
                assembly = ConfigDllManager.DllManager.LoadDll(assemblyFile, assemblyPath);
                RecordLoadedAssembly(assemblyPath, assembly);
            }


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

        public void UnloadModule(string moduleName) {
            if (!_loadedModules.TryGetValue(moduleName, out var moduleInfo))
            {
                throw new InvalidOperationException($"Module {moduleName} is not loaded.");
            }

            // 检查是否有其他模块依赖此模块
            var dependentModules = _loadedModules.Values
                .Where(m => m.DependsOn.Contains(moduleName))
                .ToList();

            if (dependentModules.Any())
            {
                throw new InvalidOperationException(
                    $"Cannot unload module {moduleName} because it is required by: {string.Join(", ", dependentModules.Select(m => m.ModuleName))}");
            }

            // 清理模块资源
            CleanupModule(moduleName);

            // 从已加载模块列表中移除
            _loadedModules.Remove(moduleName);
        }

        public void UnloadModule(string assembly, string moduleName) {
            if (!_loadedAssemblies.ContainsKey(assembly))
            {
                throw new InvalidOperationException($"Assembly {assembly} is not loaded.");
            }

            UnloadModule(moduleName);
        }

        public void CleanupModule(string moduleName) {
            if (!_loadedModules.TryGetValue(moduleName, out var moduleInfo))
            {
                throw new InvalidOperationException($"Module {moduleName} is not loaded.");
            }

            try
            {
                // 如果模块实现了 IDisposable，调用 Dispose 方法
                if (moduleInfo.Instance is IDisposable disposable)
                {
                    disposable.Dispose();
                }

                // 如果模块实现了自定义的清理接口，调用清理方法
                if (moduleInfo.Instance is IModuleCleanup cleanup)
                {
                    cleanup.Cleanup();
                }

                // 从容器中移除模块的服务注册
                if (moduleInfo.Instance is IModuleShutdown module)
                {
                    module.OnShutdown(_containerRegistry);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error cleaning up module {moduleName}: {ex.Message}");
                throw;
            }
        }

        public void CleanupAllModules() {
            // 按依赖关系的反序清理模块
            var sortedModules = _loadedModules.Values
                    .OrderByDescending<ModuleInfo, int>(m => m.DependsOn.Count())
                ;

            foreach (var moduleInfo in sortedModules)
            {
                try
                {
                    CleanupModule(moduleInfo.ModuleName);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error cleaning up module {moduleInfo.ModuleName}: {ex.Message}");
                    // 继续清理其他模块
                }
            }

            _loadedModules.Clear();
            _loadedAssemblies.Clear();
            ConfigDllManager.DllManager.UnloadAll();
        }

        public bool IsModuleLoaded(string moduleName) {
            return _loadedModules.ContainsKey(moduleName);
        }

        public IEnumerable<ModuleInfo> GetLoadedModules() {
            return _loadedModules.Values;
        }
    }
}