using CommunityToolkit.Mvvm.ComponentModel;
using XPrism.Core.BindableBase;
using XPrism.Core.DI;
using XPrism.Core.Events;

namespace ResetResourcesModules.ViewModel;

[AutoRegister(ServiceLifetime.Singleton, nameof(ResetViewModel))]
public partial class ResetViewModel : ViewModelBase {
    
    public ResetViewModel(IEventAggregator eventAggregator) : base(eventAggregator) {
        Message = "Reset";
    }
    
    [ObservableProperty] private string _message;
}