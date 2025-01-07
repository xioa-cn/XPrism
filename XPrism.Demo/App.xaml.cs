using System.Reflection;
using System.Windows;
using XPrism.Core.DI;
using XPrism.Core.Events;
using XPrism.Demo.ViewModels;

namespace XPrism.Demo;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application {
    public static IContainerRegistry ContainerLocator = XPrism.Core.DI.ContainerLocator.Container;


    protected override void OnStartup(StartupEventArgs e) {
        ContainerLocator.RegisterSingleton<IEventAggregator, EventAggregator>();
        ContainerLocator.AutoRegisterByAttribute(Assembly.Load("XPrism.Demo"));
        ContainerLocator.Build();
        var model = ContainerLocator.GetService<PubViewModel>(nameof(PubViewModel));
        model.Content = "Conte";
        var model1 = ContainerLocator.GetService<PubViewModel>(nameof(PubViewModel));
        base.OnStartup(e);
    }
}