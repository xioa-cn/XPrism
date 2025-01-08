using XPrism.Core.DI;
using XPrism.Core.Modules;
using XPrism.Core.Modules.Find;

namespace HomeModules;

[Module(nameof(Home1Module))]
public class Home1Module : IModule {
    public void RegisterTypes(IContainerRegistry containerRegistry) {
        
    }

    public void OnInitialized(IContainerProvider containerProvider) {
        
    }
}