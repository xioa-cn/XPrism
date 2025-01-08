using MainModules.ViewModel;
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
    }

    public void OnInitialized(IContainerProvider containerProvider) {
    }
}