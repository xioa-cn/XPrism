using System.Reflection;
using System.Windows;
using MainModules.ViewModel;
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
    public static IContainerRegistry ContainerLocator = XPrism.Core.DI.ContainerLocator.Container;


    protected override void OnStartup(StartupEventArgs e) {
        base.OnStartup(e);
        ContainerLocator.Initialized()
            .RegisterSingleton<IEventAggregator, EventAggregator>();
        ContainerLocator.AutoRegisterByAttribute(Assembly.Load("XPrism.Demo"));

        // 1. 配置容器
        ContainerLocator
            .RegisterSingleton<IEventAggregator, EventAggregator>()
            .RegisterSingleton<IModuleFinder>(new DirectoryModuleFinder())
            .RegisterMeModuleManager(manager => { manager.LoadModulesConfig(AppDomain.CurrentDomain.BaseDirectory); });

        ContainerLocator.RegisterSingleton<UserInfo>(new UserInfo(), nameof(UserInfo));

        // // 2. 加载模块
        // var moduleManager = ContainerLocator.GetService<IModuleManager>();
        // moduleManager.LoadModules();

        ContainerLocator.AutoRegisterByAttribute<XPrismViewModelAttribute>(Assembly.Load("XPrism.Demo"));
        ContainerLocator.Build();

        var u = ContainerLocator.GetService("MainViewModel");

        var window = App.ContainerLocator.GetService(nameof(MainWindow)) as Window;
        if (window is null)
            throw new NullReferenceException();
        window.Show();
    }
}