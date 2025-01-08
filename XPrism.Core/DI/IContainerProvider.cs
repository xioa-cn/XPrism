namespace XPrism.Core.DI
{
    /// <summary>
    /// 定义容器提供者的接口
    /// </summary>
    public interface IContainerProvider
    {
        /// <summary>
        /// 解析服务
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <returns>服务实例</returns>
        T? Resolve<T>();

        /// <summary>
        /// 解析服务
        /// </summary>
        /// <param name="type">服务类型</param>
        /// <returns>服务实例</returns>
        object? Resolve(Type type);

        /// <summary>
        /// 尝试解析服务
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <param name="instance">解析出的实例</param>
        /// <returns>是否成功解析</returns>
        bool TryResolve<T>(out T? instance);

        /// <summary>
        /// 尝试解析服务
        /// </summary>
        /// <param name="type">服务类型</param>
        /// <param name="instance">解析出的实例</param>
        /// <returns>是否成功解析</returns>
        bool TryResolve(Type type, out object? instance);
    }
} 