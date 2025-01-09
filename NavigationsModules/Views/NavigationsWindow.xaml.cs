using System.Windows;
using NavigationsModules.ViewModel;
using XPrism.Core.DataContextWindow;

namespace NavigationsModules.Views;

[XPrismViewModel(nameof(NavigationsWindow), typeof(NavigationsViewModel), nameof(NavigationsViewModel))]
public partial class NavigationsWindow : XPrismWindow {
    public NavigationsWindow() {
        InitializeComponent();
    }
}