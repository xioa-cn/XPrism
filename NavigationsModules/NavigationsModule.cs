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
                // 无 ViewModel 形式 路由为 region/{ViewName}
                regionManager.RegisterViewWithRegion<HomeView>("MainRegion","Ho");


                //导航的界面可以是Page 或者UserControl
                //导航界面的View 可以选择继承 INavigationAware 也可以不继承 
                // 导航相关也不需要额外的容器装配代码 只需要使用regionManager.Register注册为导航即可  - 包含 View ViewModel 如下例子
                regionManager.RegisterForNavigation<AboutView, AboutViewModel>("MainRegion","About1View");
                
                regionManager.RegisterViewWithRegion<About01>("AboutRegion");
                regionManager.RegisterViewWithRegion<About02>("AboutRegion");
            });
    }

    public void OnInitialized(IContainerProvider containerProvider) {
    }
}