namespace XPrism.Core.DI
{
    /// <summary>
    /// 容器注册表的实现
    /// </summary>
    internal class ContainerRegistry : IContainerRegistry
    {
        private readonly IContainerExtension<IServiceProvider> _container;
        private readonly HashSet<(Type Type, string Name)> _namedServices = new();
        private IServiceProvider? _builtContainer;

        public ContainerRegistry()
        {
            _container = new MicrosoftDependencyInjectionContainerExtension();
        }

        public IServiceProvider Container => _builtContainer ??= Build();

        public IServiceProvider Build()
        {
            if (_builtContainer != null)
            {
                return _builtContainer;
            }

            _container.FinalizeExtension();
            _builtContainer = _container.Instance;
            return _builtContainer;
        }

        private void ValidateNamedService(Type type, string name)
        {
            var key = (type, name);
            if (_namedServices.Contains(key))
            {
                throw new InvalidOperationException(
                    $"Service of type {type.Name} with name '{name}' is already registered.");
            }
            _namedServices.Add(key);
        }

        public IContainerRegistry RegisterTransient(Type from, Type to)
        {
            _container.RegisterTransient(from, to);
            return this;
        }

        public IContainerRegistry RegisterTransient(Type from, Type to, string name)
        {
            ValidateNamedService(from, name);
            _container.RegisterTransient(from, to, name);
            return this;
        }

        public IContainerRegistry RegisterInstance(Type type, object instance)
        {
            _container.RegisterInstance(type, instance);
            return this;
        }

        public IContainerRegistry RegisterSingleton(Type from, Type to)
        {
            _container.RegisterSingleton(from, to);
            return this;
        }
        
        public IContainerRegistry RegisterSingleton(Type from, Type to, string name)
        {
            ValidateNamedService(from, name);
            _container.RegisterSingleton(from, to, name);
            return this;
        }

        public IContainerRegistry RegisterScoped(Type from, Type to)
        {
            _container.RegisterScoped(from, to);
            return this;
        }

        public IContainerRegistry RegisterScoped(Type from, Type to, string name)
        {
            ValidateNamedService(from, name);
            _container.RegisterScoped(from, to, name);
            return this;
        }

        public object Resolve(Type type)
        {
            return _container.Resolve(type);
        }

        public object ResolveNamed(Type type, string name)
        {
            return _container.ResolveNamed(type, name);
        }

        public IContainerExtension GetContainer()
        {
            _container.FinalizeExtension();
            return _container;
        }

        public IServiceScope CreateScope()
        {
            return _container.CreateScope();
        }

        public T GetService<T>(string serviceName)
        {
            try
            {
                return (T)ResolveNamed(typeof(T), serviceName);
            }
            catch (KeyNotFoundException)
            {
                // 如果找不到命名服务，尝试使用默认注册
                return (T)Resolve(typeof(T));
            }
        }

        public T GetService<T>()
        {
            return (T)Resolve(typeof(T));
        }

        public object GetService(Type serviceType, string serviceName)
        {
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

        public object GetService(Type serviceType)
        {
            return Resolve(serviceType);
        }
    }
} 