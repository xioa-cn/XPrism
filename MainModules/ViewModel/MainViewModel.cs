using CommunityToolkit.Mvvm.ComponentModel;
using XPrism.Core.BindableBase;

namespace MainModules.ViewModel;

public partial class MainViewModel : ViewModelBase {
    [ObservableProperty] private string _title;

    public MainViewModel() {
        Title = "Main Module";
    }
}