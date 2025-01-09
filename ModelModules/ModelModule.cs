using XPrism.Core.DI;
using XPrism.Core.Modules;
using XPrism.Core.Modules.Find;

namespace ModelModules;

[Module(nameof(ModelModule))]
public class ModelModule : IModule {
    public void RegisterTypes(IContainerRegistry containerRegistry) {
    }

    public void OnInitialized(IContainerProvider containerProvider) {
    }
}