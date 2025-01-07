using System.Windows;
using XPrism.Core.DataContextWindow;
using XPrism.Demo.ViewModels;

namespace XPrism.Demo.Views;

[XPrismViewModel(nameof(PubWindow),typeof(PubViewModel), nameof(PubViewModel))]
public partial class PubWindow : XPrismWindow {
    public PubWindow() {
        InitializeComponent();
    }
}