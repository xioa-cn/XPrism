using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XPrism.Core.BindableBase;
using XPrism.Core.DI;
using XPrism.Core.Navigations;

namespace NavigationsModules.ViewModel;

/// <summary>
/// 导航的ViewModel不需要手动注册 也不需要特性注册
/// 在 regionManager.RegisterNavigation 的时候 ViewModel 会被直接注册进容器
/// </summary>
public partial class AboutViewModel : ViewModelBase {
    [ObservableProperty] private string _title;

    private readonly INavigationService _navigationService;
    
    public AboutViewModel(INavigationService navigationService) {
        _navigationService = navigationService;
        Title = "About";
    }
    
    [RelayCommand]
    public async Task NavigateToPage1() {
        await _navigationService.NavigateAsync("AboutRegion/About01");
    }

    [RelayCommand]
    public async Task NavigateToPage2() {
        await _navigationService.NavigateAsync("AboutRegion/About02");
    }
}