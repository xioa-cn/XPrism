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
using XPrism.Demo.Views;

namespace XPrism.Demo;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {
    public MainWindow() {
        InitializeComponent();
    }

    private void PubWindow_Show(object sender, RoutedEventArgs e) {
        PubWindow window = new PubWindow();
        window.Show();
    }

    private void SubWindow_Show(object sender, RoutedEventArgs e) {
        SubWindow window = new SubWindow();
        window.Show();
    }
}