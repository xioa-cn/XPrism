using System.Windows.Controls;
using XPrism.Core.Navigations;

namespace NavigationsModules.Pages.Components;

public partial class About01 : UserControl,INavigationAware {
    public About01() {
        InitializeComponent();
    }

    public async Task OnNavigatingToAsync(INavigationParameters parameters) {
        
    }

    public async Task OnNavigatingFromAsync(INavigationParameters parameters) {
        
    }

    public async Task<bool> CanNavigateToAsync(INavigationParameters parameters) {
        return true;
    }

    public async Task<bool> CanNavigateFromAsync(INavigationParameters parameters) {
        return true;
    }
}