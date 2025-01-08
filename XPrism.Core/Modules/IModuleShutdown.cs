using XPrism.Core.DI;

namespace XPrism.Core.Modules;

public interface IModuleShutdown {
    void OnShutdown(IContainerRegistry containerRegistry);
}