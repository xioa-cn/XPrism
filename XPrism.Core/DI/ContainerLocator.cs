using System.Reflection;

namespace XPrism.Core.DI
{
    /// <summary>
    /// 容器定位器，提供对容器的全局访问
    /// </summary>
    public static class ContainerLocator
    {
        private static IContainerRegistry? _container;
        private static readonly object _lockObject = new();

        /// <summary>
        /// 获取容器实例
        /// </summary>
        public static IContainerRegistry Container
        {
            get
            {
                if (_container == null)
                {
                    lock (_lockObject)
                    {
                        _container ??= new ContainerRegistry();
                    }
                }
                return _container;
            }
        }

        /// <summary>
        /// 设置容器实例
        /// </summary>
        /// <param name="container">容器实例</param>
        internal static void SetContainer(IContainerRegistry container)
        {
            lock (_lockObject)
            {
                _container = container;
            }
        }

        /// <summary>
        /// 重置容器实例
        /// </summary>
        public static void Reset()
        {
            lock (_lockObject)
            {
                _container = null;
            }
        }

        /// <summary>
        /// 自动注册带有AutoRegisterAttribute特性的类型
        /// </summary>
        /// <param name="assemblies">要扫描的程序集，如果为空则扫描当前程序集</param>
        /// <returns>容器注册表实例</returns>
        public static IContainerRegistry AutoRegister(params Assembly[] assemblies)
        {
            return Container.AutoRegister(
                assemblies.Length == 0 ? new[] { Assembly.GetExecutingAssembly() } : assemblies,
                type => type.GetCustomAttribute<AutoRegisterAttribute>() != null);
        }

        /// <summary>
        /// 自动注册指定命名空间下的类型
        /// </summary>
        /// <param name="namespacePrefix">命名空间前缀</param>
        /// <returns>容器注册表实例</returns>
        public static IContainerRegistry AutoRegisterNamespace(string namespacePrefix)
        {
            return Container.AutoRegisterByNamespace(namespacePrefix);
        }

        /// <summary>
        /// 自动注册满足条件的类型
        /// </summary>
        /// <param name="filter">类型过滤器</param>
        /// <returns>容器注册表实例</returns>
        public static IContainerRegistry AutoRegisterWhere(Func<Type, bool> filter)
        {
            return Container.AutoRegister(filter);
        }

        /// <summary>
        /// 通过服务名称获取服务实例
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <param name="serviceName">服务名称</param>
        /// <returns>服务实例</returns>
        public static T GetService<T>(string serviceName)
        {
            return Container.GetService<T>(serviceName);
        }

        /// <summary>
        /// 获取服务实例
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <returns>服务实例</returns>
        public static T GetService<T>()
        {
            return Container.GetService<T>();
        }

        /// <summary>
        /// 通过服务名称获取服务实例
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceName">服务名称</param>
        /// <returns>服务实例</returns>
        public static object GetService(Type serviceType, string serviceName)
        {
            return Container.GetService(serviceType, serviceName);
        }

        /// <summary>
        /// 获取服务实例
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <returns>服务实例</returns>
        public static object GetService(Type serviceType)
        {
            return Container.GetService(serviceType);
        }
    }
} 