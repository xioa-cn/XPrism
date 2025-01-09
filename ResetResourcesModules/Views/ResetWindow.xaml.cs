using System.Windows;
using ResetResourcesModules.ViewModel;
using XPrism.Core.DataContextWindow;

namespace ResetResourcesModules.Views;

[XPrismViewModel(nameof(ResetWindow),typeof(ResetViewModel),nameof(ResetViewModel))]
public partial class ResetWindow : XPrismWindow {
    public ResetWindow() {
        InitializeComponent();
    }
}