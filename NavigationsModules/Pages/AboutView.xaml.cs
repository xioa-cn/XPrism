using System.Windows.Controls;
using XPrism.Core.Navigations;

namespace NavigationsModules.Pages;

public partial class AboutView : Page, INavigationAware {
    public AboutView() {
        InitializeComponent();
    }

    public async Task OnNavigatingToAsync(INavigationParameters parameters) {
    }

    public async Task OnNavigatingFromAsync(INavigationParameters parameters) {
    }

    public async Task<bool> CanNavigateToAsync(INavigationParameters parameters) {
        var param = parameters.GetValue<string>("key");
        return true;
    }

    public async Task<bool> CanNavigateFromAsync(INavigationParameters parameters) {
        return true;
    }
}