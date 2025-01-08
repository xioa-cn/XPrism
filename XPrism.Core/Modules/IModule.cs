using XPrism.Core.DI;

namespace XPrism.Core.Modules
{
    /// <summary>
    /// 定义模块的基本接口
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// 注册模块的服务
        /// </summary>
        /// <param name="containerRegistry">容器注册表</param>
        void RegisterTypes(IContainerRegistry containerRegistry);

        /// <summary>
        /// 模块初始化
        /// </summary>
        /// <param name="containerProvider">容器提供者</param>
        void OnInitialized(IContainerProvider containerProvider);
        
        
    }
} 