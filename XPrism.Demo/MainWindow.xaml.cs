using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XPrism.Core.DataContextWindow;
using XPrism.Core.DI;
using XPrism.Demo.Views;

namespace XPrism.Demo;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
[XPrismViewModel(nameof(MainWindow))]
public partial class MainWindow {
    public MainWindow() {
        InitializeComponent();
    }

    private void PubWindow_Show(object sender, RoutedEventArgs e) {
        //PubWindow window = new PubWindow();
        var window = XPrismIoc.Fetch(nameof(PubWindow)) as Window;
        window.Show();
    }

    private void SubWindow_Show(object sender, RoutedEventArgs e) {
        //SubWindow window = new SubWindow();
        var window = XPrismIoc.Fetch(nameof(SubWindow)) as Window;
        window.Show();
    }
}