using System.Reflection;
using System.Windows;
using MainModules.ViewModel;
using XPrism.Core.Co;
using XPrism.Core.DataContextWindow;
using XPrism.Core.DI;
using XPrism.Core.Events;
using XPrism.Core.Modules;
using XPrism.Core.Modules.Find;
using XPrism.Demo.Model;
using XPrism.Demo.ViewModels;

namespace XPrism.Demo;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application {
    protected override void OnStartup(StartupEventArgs e) {
        base.OnStartup(e);
        XPrism.Core.DI.ContainerLocator.Container.Initialized()
            .RegisterEventAggregator<EventAggregator>();
        XPrism.Core.DI.ContainerLocator.Container.AutoRegisterByAttribute(Assembly.Load("XPrism.Demo"));

        // 1. 配置容器
        XPrism.Core.DI.ContainerLocator.Container
            .RegisterSingleton<IModuleFinder>(new DirectoryModuleFinder())
            .RegisterMeModuleManager(manager => { manager.LoadModulesConfig(AppDomain.CurrentDomain.BaseDirectory); });

        XPrism.Core.DI.ContainerLocator.Container.RegisterSingleton<UserInfo>(new UserInfo(), nameof(UserInfo));

        XPrism.Core.DI.ContainerLocator.Container.AutoRegisterByAttribute<XPrismViewModelAttribute>(
            Assembly.Load("XPrism.Demo"));
        XPrism.Core.DI.ContainerLocator.Container.Build();

        var window = NavigationWindow.Fetch("NavigationsWindow");
        //var vm = XPrismIoc.Fetch("ResetViewModel");
        if (window is null)
            throw new NullReferenceException();
        //var s = DllManager.LoadedContexts;
        window.Show();
    }
}