using System.Reflection;
using System.Windows;
using XPrism.Core.DataContextWindow;
using XPrism.Core.DI;
using XPrism.Core.Events;
using XPrism.Demo.ViewModels;

namespace XPrism.Demo;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static IContainerRegistry ContainerLocator = XPrism.Core.DI.ContainerLocator.Container;


    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        ContainerLocator.RegisterSingleton<IEventAggregator, EventAggregator>();
        ContainerLocator.AutoRegisterByAttribute(Assembly.Load("XPrism.Demo"));
        ContainerLocator.AutoRegisterByAttribute<XPrismViewModelAttribute>(Assembly.Load("XPrism.Demo"));
        ContainerLocator.Build();

        var window = App.ContainerLocator.GetService(nameof(MainWindow)) as Window;
        if (window is null)
            throw new NullReferenceException();
        window.Show();
    }
}