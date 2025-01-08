using System.Reflection;
using XPrism.Core.Modules.Find;

namespace XPrism.Core.Modules {
    /// <summary>
    /// 模块管理器接口
    /// </summary>
    public interface IModuleManager {
        /// <summary>
        /// 加载所有模块
        /// </summary>
        void LoadModules();

        /// <summary>
        /// 加载指定的模块集合
        /// </summary>
        void LoadModule(IEnumerable<ModuleInfo> modules);

        /// <summary>
        /// 按需加载指定模块
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        void LoadModule(string moduleName);
        
        /// <summary>
        /// 按需加载指定模块
        /// </summary>
        /// <param name="assembly">程序集</param>
        /// <param name="moduleName">模块名</param>
        void LoadModule(string assembly,string moduleName);

        /// <summary>
        /// 卸载指定模块
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        void UnloadModule(string moduleName);

        /// <summary>
        /// 卸载指定程序集中的模块
        /// </summary>
        /// <param name="assembly">程序集</param>
        /// <param name="moduleName">模块名</param>
        void UnloadModule(string assembly, string moduleName);

        /// <summary>
        /// 清理指定模块的资源
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        void CleanupModule(string moduleName);

        /// <summary>
        /// 清理所有已加载模块的资源
        /// </summary>
        void CleanupAllModules();

        /// <summary>
        /// 检查模块是否已加载
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <returns>是否已加载</returns>
        bool IsModuleLoaded(string moduleName);

        /// <summary>
        /// 获取已加载的模块列表
        /// </summary>
        /// <returns>已加载的模块列表</returns>
        IEnumerable<ModuleInfo> GetLoadedModules();
    }
}