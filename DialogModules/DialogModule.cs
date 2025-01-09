using DialogModules.Views;
using XPrism.Core.DI;
using XPrism.Core.Dialogs;
using XPrism.Core.Modules;
using XPrism.Core.Modules.Find;

namespace DialogModules;

[Module(nameof(DialogModule))]
public class DialogModule : IModule {
    public void RegisterTypes(IContainerRegistry containerRegistry) {
        containerRegistry.RegisterDialogServiceCommonBase()
            .RegisterDialogService<ConfirmDialog, ConfirmDialogView>()
            .RegisterDialogService<MessageDialog, MessageDialogView>()
            .RegisterDialogService<SubmitDialog, SubmitDialogView>();
    }

    public void OnInitialized(IContainerProvider containerProvider) {
    }
}