namespace XPrism.Core.Modules.Find {
    /// <summary>
    /// 模块信息
    /// </summary>
    public class ModuleInfo {
        /// <summary>
        /// 模块实例
        /// </summary>
        public object Instance { get; set; }

        /// <summary>
        /// 模块类型
        /// </summary>
        public Type ModuleType { get; }

        /// <summary>
        /// 模块名称
        /// </summary>
        public string ModuleName { get; }

        /// <summary>
        /// 是否按需加载
        /// </summary>
        public bool IsOnDemand { get; }

        /// <summary>
        /// 依赖的模块名称列表
        /// </summary>
        public IEnumerable<string> DependsOn { get; }

        /// <summary>
        /// 模块状态
        /// </summary>
        public ModuleState State { get; set; } = ModuleState.NotLoaded;

        /// <summary>
        /// 初始化模块信息
        /// </summary>
        public ModuleInfo(
            Type moduleType,
            string moduleName,
            bool isOnDemand = false,
            IEnumerable<string>? dependsOn = null) {
            ModuleType = moduleType;
            ModuleName = moduleName;
            IsOnDemand = isOnDemand;
            DependsOn = dependsOn ?? Array.Empty<string>();
        }
    }
}