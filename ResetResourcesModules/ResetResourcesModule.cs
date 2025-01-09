using System.Reflection;
using XPrism.Core.DataContextWindow;
using XPrism.Core.DI;
using XPrism.Core.Modules;
using XPrism.Core.Modules.Find;

namespace ResetResourcesModules;

[Module(nameof(ResetResourcesModule))]
public class ResetResourcesModule : IModule {
    public void RegisterTypes(IContainerRegistry containerRegistry) {
        ContainerLocator.Container.AutoRegisterByAttribute(Assembly.Load("ResetResourcesModules"));
        ContainerLocator.Container.AutoRegisterByAttribute<XPrismViewModelAttribute>(
            Assembly.Load("ResetResourcesModules"));
    }

    public void OnInitialized(IContainerProvider containerProvider) {
    }
}