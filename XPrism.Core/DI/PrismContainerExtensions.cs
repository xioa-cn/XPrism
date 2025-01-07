namespace XPrism.Core.DI
{
    /// <summary>
    /// Prism容器扩展方法类
    /// </summary>
    public static class PrismContainerExtensions
    {
        /// <summary>
        /// 使用Microsoft依赖注入容器
        /// </summary>
        /// <param name="containerRegistry">容器注册表</param>
        /// <returns>容器注册表实例</returns>
        public static IContainerRegistry UseMicrosoftDependencyInjection(this IContainerRegistry containerRegistry)
        {
            var container = new ContainerRegistry();
            container.RegisterInstance(typeof(IContainerExtension), container);
            ContainerLocator.SetContainer(container);
            return container;
        }
    }
} 