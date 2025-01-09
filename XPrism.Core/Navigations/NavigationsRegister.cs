using XPrism.Core.DI;

namespace XPrism.Core.Navigations;

public static class NavigationsRegister {
    public static IContainerRegistry AddNavigations(this IContainerRegistry containerRegistry,
        Action<IRegionManager>? registerAction = null) {
        return containerRegistry.RegistryNavigations(registerAction);
    }
}