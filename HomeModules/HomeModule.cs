using System.Reflection;
using HomeModules.ViewModel;
using XPrism.Core.DataContextWindow;
using XPrism.Core.DI;
using XPrism.Core.Modules;
using XPrism.Core.Modules.Find;

namespace HomeModules;

[Module("HomeModule")]
public class HomeModule : IModule {
    public void RegisterTypes(IContainerRegistry containerRegistry) {
        containerRegistry.AutoRegisterByAttribute<XPrismViewModelAttribute>(Assembly.Load("HomeModules"));
        containerRegistry.RegisterSingleton<HomeViewModel>(nameof(HomeViewModel));
    }

    public void OnInitialized(IContainerProvider containerProvider) {
    }
}