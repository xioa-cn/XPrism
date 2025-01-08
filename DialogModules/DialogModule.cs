using XPrism.Core.DI;
using XPrism.Core.Dialogs;
using XPrism.Core.Modules;
using XPrism.Core.Modules.Find;

namespace DialogModules;

[Module(nameof(DialogModule))]
public class DialogModule : IModule {
    public void RegisterTypes(IContainerRegistry containerRegistry) {
        containerRegistry.RegisterSingleton<IDialogService, DialogService>();
        containerRegistry.RegisterSingleton<IDialogPresenter, DialogPresenter>();
        containerRegistry.RegisterTransient<ConfirmDialogView>();
        containerRegistry.RegisterTransient<XPrism.Core.Dialogs.MessageDialogView>();
    }

    public void OnInitialized(IContainerProvider containerProvider) {
        
    }
}