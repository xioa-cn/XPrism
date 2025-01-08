namespace XPrism.Core.Modules.Find;

/// <summary>
/// 模块状态枚举
/// </summary>
public enum ModuleState
{
    NotLoaded,
    Loading,
    Loaded,
    Initializing,
    Initialized,
    Error
}