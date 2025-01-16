using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using XPrism.Core.DebugLog;

namespace XPrism.Core.DI {
    /// <summary>
    /// Microsoft DI容器的实现
    /// </summary>
    public class MicrosoftDependencyInjectionContainerExtension : IContainerExtension<IServiceProvider> {
        /// <summary>
        /// 存储类型注册信息的字典
        /// </summary>
        private readonly Dictionary<Type, ServiceDescriptor> _services;

        /// <summary>
        /// 存储命名服务注册信息的字典
        /// </summary>
        private readonly Dictionary<(Type, string), ServiceDescriptor> _namedServices;

        /// <summary>
        /// 存储单例实例的字典
        /// </summary>
        private readonly Dictionary<Type, object> _instances;

        private readonly Dictionary<Type, ServiceDescriptor> _scopedServices = new();
        private readonly Dictionary<IServiceScope, Dictionary<Type, object>> _scopedInstances = new();

        public MicrosoftDependencyInjectionContainerExtension() {
            _services = new Dictionary<Type, ServiceDescriptor>();
            _namedServices = new Dictionary<(Type, string), ServiceDescriptor>();
            _instances = new Dictionary<Type, object>();
        }

        public IServiceProvider? Instance { get; private set; }

        public void FinalizeExtension() {
            Instance = new ServiceProvider(this);
        }

        public void RegisterTransient(Type from, Type to) {
            _services[from] = new ServiceDescriptor(from, to, ServiceLifetime.Transient);
        }

        public void RegisterTransient(Type from, Type to, string name) {
            _namedServices[(from, name)] = new ServiceDescriptor(from, to, ServiceLifetime.Transient);
        }

        public void RegisterInstance(Type type, object instance) {
            _instances[type] = instance;
        }

        /// <summary>
        /// 注册命名服务实例
        /// </summary>
        /// <param name="type">服务类型</param>
        /// <param name="instance">服务实例</param>
        /// <param name="name">服务名称</param>
        public void RegisterInstance(Type type, object instance, string? name) {
            var descriptor = new ServiceDescriptor(type, instance, ServiceLifetime.Singleton);

            if (name == null)
            {
                _services[type] = descriptor;
                _instances[type] = instance;
            }
            else
            {
                _namedServices[(type, name)] = descriptor;
                _instances[type] = instance;
            }
        }

        public void RegisterSingleton(Type from, Type to) {
            _services[from] = new ServiceDescriptor(from, to, ServiceLifetime.Singleton);
        }

        public void RegisterSingleton<T>(Type from, Type to, Action<T>? registerAction) {
            if (registerAction is not null)
            {
                // 将泛型 Action<T> 转换为 Action<object>
                Action<object> wrappedAction = obj =>
                {
                    if (obj is T typedInstance)
                    {
                        registerAction(typedInstance);
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            $"Cannot configure service: instance is not of type {typeof(T).Name}");
                    }
                };
                var service = new ServiceDescriptor(from, to, ServiceLifetime.Singleton, wrappedAction);
                _services[from] = service;

                CreateInstance(service);
            }
            else
            {
                _services[from] = new ServiceDescriptor(from, to, ServiceLifetime.Singleton);
            }
        }

        public void RegisterSingleton(Type from, Type to, string name) {
            DebugLogger.LogInfo($"RegisterSingleton Type {from} to {to} name {name}");
            _namedServices[(from, name)] = new ServiceDescriptor(from, to, ServiceLifetime.Singleton);
        }


        public object? Resolve(Type type) {
            if (_instances.TryGetValue(type, out var instance))
            {
                DebugLogger.LogInfo($"Resolved instance of type {type.Name}");
                return instance;
            }

            if (_services.TryGetValue(type, out var descriptor))
            {
                if (descriptor.Lifetime == ServiceLifetime.Scoped)
                {
                    throw new InvalidOperationException(
                        $"Cannot resolve scoped service '{type.Name}' from root provider.");
                }

                DebugLogger.LogInfo($"Resolved instance of type {type.Name} from root provider.");
                return CreateInstance(descriptor);
            }

            var gethavingServices = _namedServices
                .FirstOrDefault(e => e.Key.Item1 == type);

            if (gethavingServices.Key.Item2 != null)
            {
                return Resolve(gethavingServices.Key.Item2);
            }


            throw new KeyNotFoundException($"Type {type.Name} is not registered");
        }

        public T? Resolve<T>() {
            return (T)Resolve(typeof(T));
        }

        public bool TryResolve<T>(out T? instance) {
            var value = Resolve(typeof(T));

            if (value is T v)
            {
                instance = v;
                return true;
            }

            instance = default(T);
            return false;
        }

        public bool TryResolve(Type type, out object? instance) {
            var value = Resolve(type);

            if (value is not null)
            {
                instance = value;
                return true;
            }

            instance = null;
            return false;
        }

        public object? Resolve(string name) {
            var service = _namedServices?.FirstOrDefault(e => e.Key.Item2 == name);
            return service is not null ? CreateInstance(service.Value.Value) : null;
        }


        public void ResetService(Type type) {
            var find =
                _services.FirstOrDefault(e => e.Key == type);
            if (find.Value is not null)
            {
                //find.Value.Instance = null;
                find.Value.CachedInstance = null;
            }
            else
            {
                throw new KeyNotFoundException($"Type {type} is not registered");
            }
        }

        public object ResolveNamed(Type type, string name) {
            if (_namedServices.TryGetValue((type, name), out var descriptor))
            {
                if (descriptor.Lifetime == ServiceLifetime.Scoped)
                {
                    throw new InvalidOperationException(
                        $"Cannot resolve scoped service '{type.Name}' with name '{name}' from root provider.");
                }

                DebugLogger.LogInfo($"Resolved instance of type {type.Name} from root provider.");
                return CreateInstance(descriptor);
            }

            throw new KeyNotFoundException($"Type {type.Name} with name '{name}' is not registered");
        }

        public void ResetService(string name) {
            var service = _namedServices?.FirstOrDefault(e => e.Key.Item2 == name);
            if (service is null)
            {
                throw new KeyNotFoundException($"Type {name} is not registered");
            }

            if (service.Value.Value.Instance is not null)
                throw new Exception("Cannot reset service Instance");
            DebugLogger.LogInfo($"Resetting instance of type {service.Value.Key.Item2} from root provider.");
            service.Value.Value.CachedInstance = null;
        }

        private object CreateInstance(ServiceDescriptor descriptor, bool action = false) {
            DebugLogger.LogInfo($"Get of {descriptor.ServiceType.FullName}");
            if (descriptor.Instance != null)
            {
                DebugLogger.LogInfo($"{descriptor.ServiceType.FullName} is in Instance");
                return descriptor.Instance;
            }


            if (descriptor.Lifetime == ServiceLifetime.Singleton && descriptor.Instance != null)
            {
                //DebugLogger.LogInfo($"{descriptor.ServiceType.FullName} is in Instance");
                return descriptor.Instance;
            }

            if (descriptor.Lifetime == ServiceLifetime.Singleton && descriptor.CachedInstance != null)
            {
                DebugLogger.LogInfo($"{descriptor.ServiceType.FullName} is in CachedInstance");
                return descriptor.CachedInstance;
            }

            if (descriptor.ImplementationType == typeof(IContainerProvider))
            {
                return this;
            }

            // 获取所有构造函数
            var constructors = descriptor.ImplementationType.GetConstructors();
            if (!constructors.Any())
            {
                // 如果没有公共构造函数，尝试创建实例
                var instance = Activator.CreateInstance(descriptor.ImplementationType);
                DebugLogger.LogInfo($"{descriptor.ServiceType.FullName} is in default constructor");
                if (instance == null)
                {
                    throw new InvalidOperationException(
                        $"Failed to create instance of type {descriptor.ImplementationType.Name}");
                }

                if (descriptor.Lifetime == ServiceLifetime.Singleton)
                {
                    descriptor.CachedInstance = instance;
                }

                return instance;
            }

            // 按参数数量降序排序，优先使用参数最多的构造函数
            var constructor = constructors
                .OrderByDescending(c => c.GetParameters().Length)
                .First();

            var parameters = constructor.GetParameters();
            var parameterInstances = new object[parameters.Length];
            DebugLogger.LogInfo(
                $"{descriptor.ServiceType.FullName} is in constructor on type {parameterInstances.Length} parameters");
            // 解析构造函数参数
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                // 使用容器解析参数
                if (_services.TryGetValue(parameter.ParameterType, out var parameterDescriptor))
                {
                    DebugLogger.LogInfo($"TryGetValue : parameter {parameter.Name} in _services.");
                    parameterInstances[i] = CreateInstance(parameterDescriptor);
                }
                else if (_instances.TryGetValue(parameter.ParameterType, out var instanceDescriptor))
                {
                    DebugLogger.LogInfo($"TryGetValue : parameter {parameter.Name} in _instances.");
                    parameterInstances[i] = instanceDescriptor;
                }
                else
                {
                    // 如果参数类型未注册，尝试创建默认实例
                    try
                    {
                        DebugLogger.LogInfo($"TryCreateInstance : parameter {parameterInstances[i]}");
                        parameterInstances[i] = Activator.CreateInstance(parameter.ParameterType)
                                                ?? throw new InvalidOperationException(
                                                    $"Failed to create instance of parameter type {parameter.ParameterType.Name}");
                    }
                    catch
                    {
                        throw new InvalidOperationException(
                            $"Cannot resolve parameter of type {parameter.ParameterType.Name} " +
                            $"for {descriptor.ImplementationType.Name}. Make sure it is registered in the container.");
                    }
                }
            }

            // 创建实例
            //var result = constructor.Invoke(parameterInstances);
            var result = Activator.CreateInstance(descriptor.ImplementationType, parameterInstances);
            // 如果是单例，缓存实例
            if (descriptor.Lifetime == ServiceLifetime.Singleton)
            {
                DebugLogger.LogInfo($"Cache {descriptor.ServiceType.FullName} is in CachedInstance");
                descriptor.CachedInstance = result;
                if (descriptor.RegisterAction != null)
                {
                    descriptor.RegisterAction(result);
                }
            }

            return result;
        }

        public void RegisterScoped(Type from, Type to) {
            _scopedServices[from] = new ServiceDescriptor(from, to, ServiceLifetime.Scoped);
        }

        public void RegisterScoped(Type from, Type to, string name) {
            _namedServices[(from, name)] = new ServiceDescriptor(from, to, ServiceLifetime.Scoped);
        }

        public IServiceScope CreateScope() {
            return new ServiceScope(this);
        }

        private class ServiceScope : IServiceScope {
            private readonly MicrosoftDependencyInjectionContainerExtension _container;
            private readonly Dictionary<(Type Type, string? Name), object> _instances = new();

            public ServiceScope(MicrosoftDependencyInjectionContainerExtension container) {
                _container = container;
            }

            public IServiceProvider ServiceProvider => new ScopedServiceProvider(this, _container);

            public void Dispose() {
                foreach (var instance in _instances.Values)
                {
                    (instance as IDisposable)?.Dispose();
                }

                _instances.Clear();
            }

            internal object GetOrCreateInstance(Type type, string? name = null) {
                var key = (type, name);
                if (_instances.TryGetValue(key, out var instance))
                {
                    return instance;
                }

                var descriptor = name != null
                    ? _container._namedServices[(type, name)]
                    : _container._services[type];

                instance = _container.CreateInstance(descriptor);
                _instances[key] = instance;
                return instance;
            }
        }

        private class ScopedServiceProvider : IServiceProvider {
            private readonly ServiceScope _scope;
            private readonly MicrosoftDependencyInjectionContainerExtension _container;

            public ScopedServiceProvider(ServiceScope scope, MicrosoftDependencyInjectionContainerExtension container) {
                _scope = scope;
                _container = container;
            }

            public object? GetService(Type serviceType) {
                if (_container._services.TryGetValue(serviceType, out var descriptor))
                {
                    return descriptor.Lifetime == ServiceLifetime.Scoped
                        ? _scope.GetOrCreateInstance(serviceType)
                        : _container.CreateInstance(descriptor);
                }

                return null;
            }
        }
    }
}