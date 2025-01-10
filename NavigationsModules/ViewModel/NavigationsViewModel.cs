using CommunityToolkit.Mvvm.Input;
using XPrism.Core.BindableBase;
using XPrism.Core.DI;
using XPrism.Core.Navigations;

namespace NavigationsModules.ViewModel;

[AutoRegister(ServiceLifetime.Singleton,nameof(NavigationsViewModel))]
public partial class NavigationsViewModel : ViewModelBase {

    private readonly INavigationService _navigationService;

    public NavigationsViewModel(INavigationService navigationService) {
        _navigationService = navigationService;
    }

    [RelayCommand]
    public async Task NavigateToPage1() {
        await _navigationService.NavigateAsync("MainRegion/About1View",new NavigationParameters() {
            {"key","value"},
            {"key2","value2"}
        });
    }

    [RelayCommand]
    public async Task NavigateToPage2() {
        await _navigationService.NavigateAsync("MainRegion/Ho");
    }
}