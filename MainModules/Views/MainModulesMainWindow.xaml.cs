using System.Windows;
using MainModules.ViewModel;
using XPrism.Core.DataContextWindow;

namespace MainModules.Views;

[XPrismViewModel(nameof(MainModulesMainWindow), typeof(MainViewModel), nameof(MainViewModel))]
public partial class MainModulesMainWindow : XPrismWindow {
    public MainModulesMainWindow() {
        InitializeComponent();
    }
}