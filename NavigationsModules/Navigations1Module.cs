using NavigationsModules.Pages.Components;
using XPrism.Core.DI;
using XPrism.Core.Modules;
using XPrism.Core.Modules.Find;
using XPrism.Core.Navigations;

namespace NavigationsModules;
[Module(nameof(Navigations1Module))]
public class Navigations1Module:IModule {
    public void RegisterTypes(IContainerRegistry containerRegistry) {
        var regionManager = XPrismIoc.Fetch<IRegionManager>();
        regionManager.RegisterViewWithRegion<About01>("AboutRegion");// 如果不给viewName这个参数 会直接使用 类名
        regionManager.RegisterViewWithRegion<About02>("AboutRegion");
    }

    public void OnInitialized(IContainerProvider containerProvider) {
        
    }
}