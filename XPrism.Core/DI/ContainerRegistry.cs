using XPrism.Core.DebugLog;
using XPrism.Core.Modules;

namespace XPrism.Core.DI {
    /// <summary>
    /// 容器注册表的实现
    /// </summary>
    internal class ContainerRegistry : IContainerRegistry {
        internal readonly IContainerExtension<IServiceProvider> _container;
        private readonly HashSet<(Type Type, string Name)> _namedServices = new();
        private IServiceProvider? _builtContainer;

        public ContainerRegistry() {
            _container = new MicrosoftDependencyInjectionContainerExtension();
        }

        public IContainerExtension<IServiceProvider> GetIContainerExtension() => _container;

        public IServiceProvider Container => _builtContainer ??= Build();

        public IServiceProvider Build() {
            if (_builtContainer != null)
            {
                return _builtContainer;
            }

            _container.FinalizeExtension();
            _builtContainer = _container.Instance;
            return _builtContainer;
        }

        private void ValidateNamedService(Type type, string name) {
            var key = (type, name);
            if (_namedServices.Contains(key))
            {
                // 允许卸载 重新注册
                _namedServices.Remove(key);
                _namedServices.Add(key);
                // throw new InvalidOperationException(
                //     $"Service of type {type.Name} with name '{name}' is already registered.");
            }

            _namedServices.Add(key);
        }

        public IContainerRegistry RegisterTransient(Type from, Type to) {
            _container.RegisterTransient(from, to);
            return this;
        }


        public IContainerRegistry RegisterTransient(Type from, Type to, string name) {
            ValidateNamedService(from, name);
            _container.RegisterTransient(from, to, name);
            return this;
        }

        public IContainerRegistry RegisterInstance(Type type, object instance) {
            _container.RegisterInstance(type, instance);
            return this;
        }

        public IContainerRegistry RegisterSingleton(Type from, Type to) {
            _container.RegisterSingleton(from, to);
            return this;
        }

        public IContainerRegistry RegisterSingleton<T>(Type from, Type to, Action<T>? registerAction) {
            _container.RegisterSingleton(from, to, registerAction);
            return this;
        }

        public IContainerRegistry Initialized() {
            _container.RegisterInstance(typeof(IContainerProvider), this.GetIContainerExtension());
            _container.RegisterInstance(typeof(IContainerRegistry), ContainerLocator.Container);
            return this;
        }

        public IContainerRegistry RegisterSingleton(Type from, Type to, string name) {
            ValidateNamedService(from, name);
            _container.RegisterSingleton(from, to, name);
            return this;
        }

        /// <summary>
        /// 注册单例服务
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <param name="value">服务实例</param>
        /// <param name="name">服务实例名称</param>
        /// <returns>容器注册表</returns>
        public IContainerRegistry RegisterSingleton<T>(T value, string? name = null) where T : class {
            _container.RegisterInstance(typeof(T), value, name);
            return this;
        }

        /// <summary>
        /// 注册作用域服务
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <param name="value">服务实例</param>
        /// <param name="name">服务实例名称</param>
        /// <returns>容器注册表</returns>
        public IContainerRegistry RegisterScoped<T>(T value, string? name = null) where T : class {
            _container.RegisterInstance(typeof(T), value, name);
            return this;
        }

        public IContainerRegistry RegisterScoped(Type from, Type to) {
            _container.RegisterScoped(from, to);
            return this;
        }

        public IContainerRegistry RegisterScoped(Type from, Type to, string name) {
            ValidateNamedService(from, name);
            _container.RegisterScoped(from, to, name);
            return this;
        }

        public object Resolve(Type type) {
            //DebugLogger.LogInfo($"Resolving type {type}");
            return _container.Resolve(type);
        }

        public object? Resolve(string serviceName) {
            //DebugLogger.LogInfo($"Resolving serviceName {serviceName}");
            return _container.Resolve(serviceName);
        }

        public object ResolveNamed(Type type, string name) {
            //DebugLogger.LogInfo($"Resolving type {type} named {name}");
            return _container.ResolveNamed(type, name);
        }

        public IContainerExtension GetContainer() {
            _container.FinalizeExtension();
            return _container;
        }

        public IServiceScope CreateScope() {
            return _container.CreateScope();
        }

        public T GetService<T>(string serviceName) {
            try
            {
                //DebugLogger.LogInfo($"Resolving serviceName {serviceName} Type {typeof(T)} :GetService<T> ");
                return (T)ResolveNamed(typeof(T), serviceName);
            }
            catch (KeyNotFoundException)
            {
                // 如果找不到命名服务，尝试使用默认注册
                return (T)Resolve(typeof(T));
            }
        }

        public T GetService<T>() {
            //DebugLogger.LogInfo($"Resolving type {typeof(T)} :GetService<T> ");
            return (T)Resolve(typeof(T));
        }

        public object? GetService(string serviceName) {
            //DebugLogger.LogInfo($"Resolving serviceName {serviceName}");
            return Resolve(serviceName);
        }

        public void ResetService(string serviceName) {
            DebugLogger.LogInfo($"Resetting serviceName {serviceName}");
            _container.ResetService(serviceName);
        }

        public void ResetService(Type type) {
            DebugLogger.LogInfo($"Resetting Type {type}");
            _container.ResetService(type);
        }

        public object GetService(Type serviceType, string serviceName) {
            try
            {
                return ResolveNamed(serviceType, serviceName);
            }
            catch (KeyNotFoundException)
            {
                // 如果找不到命名服务，尝试使用默认注册
                return Resolve(serviceType);
            }
        }

        public object GetService(Type serviceType) {
            return Resolve(serviceType);
        }
    }
}