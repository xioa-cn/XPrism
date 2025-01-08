using System.Reflection;
using MainModules.ViewModel;
using XPrism.Core.DataContextWindow;
using XPrism.Core.DI;
using XPrism.Core.Events;
using XPrism.Core.Modules;
using XPrism.Core.Modules.Find;

namespace MainModules;

[Module(nameof(MainModules), DependsOn = ["HomeModule"])]
public class MainModules : IModule {
    private readonly IEventAggregator _eventAggregator;

    public MainModules(IEventAggregator eventAggregator) {
        _eventAggregator = eventAggregator;
    }

    public void RegisterTypes(IContainerRegistry containerRegistry) {
        containerRegistry.RegisterSingleton<MainViewModel>(nameof(MainViewModel));
        containerRegistry.AutoRegisterByAttribute<XPrismViewModelAttribute>(Assembly.Load("MainModules"));
    }

    public void OnInitialized(IContainerProvider containerProvider) {
    }
}