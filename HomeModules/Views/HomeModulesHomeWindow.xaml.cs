using System.Windows;
using HomeModules.ViewModel;
using XPrism.Core.DataContextWindow;

namespace HomeModules.Views;

[XPrismViewModel(nameof(HomeModulesHomeWindow), typeof(HomeViewModel), nameof(HomeViewModel))]
public partial class HomeModulesHomeWindow : XPrismWindow {
    public HomeModulesHomeWindow() {
        InitializeComponent();
    }
}