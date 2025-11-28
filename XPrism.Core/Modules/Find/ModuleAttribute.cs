namespace XPrism.Core.Modules.Find
{
    /// <summary>
    /// 模块特性，用于标记和配置模块
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ModuleAttribute : System.Attribute
    {
        /// <summary>
        /// 模块名称
        /// </summary>
        public string ModuleName { get; }

        /// <summary>
        /// 是否按需加载
        /// </summary>
        public bool IsOnDemand { get; set; }

        /// <summary>
        /// 依赖的模块名称列表
        /// </summary>
        public string[]? DependsOn { get; set; }

        public ModuleAttribute(string moduleName)
        {
            ModuleName = moduleName;
            DependsOn = Array.Empty<string>();
        }
    }
} 