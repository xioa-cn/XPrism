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

        public void LoadModule(IEnumerable<ModuleInfo> modules);

        /// <summary>
        /// 按需加载指定模块
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        void LoadModule(string moduleName);
        
        
        void LoadModule(string assembly,string moduleName);
    }
}