using System.ComponentModel;
using System.Reflection;
using NavigationsModules.Pages;
using NavigationsModules.Pages.Components;
using NavigationsModules.ViewModel;
using XPrism.Core.DataContextWindow;
using XPrism.Core.DI;
using XPrism.Core.Modules;
using XPrism.Core.Modules.Find;
using XPrism.Core.Navigations;

namespace NavigationsModules;

[Module(nameof(NavigationsModule))]
public class NavigationsModule : IModule {
    public void RegisterTypes(IContainerRegistry containerRegistry) {
        containerRegistry.AutoRegisterByAttribute(Assembly.Load("NavigationsModules"))
            .AutoRegisterByAttribute<XPrismViewModelAttribute>(Assembly.Load("NavigationsModules"));
        containerRegistry
            .RegisterSingleton<INavigationService, NavigationService>();
        containerRegistry .AddNavigations(regionManager =>
            {
                // 无 ViewModel 形式 路由为 {regionName}/{ViewName}
              
               
            });
    }

    public void OnInitialized(IContainerProvider containerProvider) {
    }
}