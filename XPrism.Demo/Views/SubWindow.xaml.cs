using System.Windows;
using XPrism.Core.DataContextWindow;
using XPrism.Demo.ViewModels;

namespace XPrism.Demo.Views;

[XPrismViewModel(nameof(SubWindow),typeof(SubViewModel), nameof(SubViewModel))]
public partial class SubWindow : XPrismWindow {
    public SubWindow() {
        InitializeComponent();
    }
}