namespace XPrism.Core.Modules.Find
{
    /// <summary>
    /// 模块发现接口
    /// </summary>
    public interface IModuleFinder
    {
        /// <summary>
        /// 查找所有模块
        /// </summary>
        /// <returns>模块信息列表</returns>
        IEnumerable<ModuleInfo> FindModules();
    }
} 