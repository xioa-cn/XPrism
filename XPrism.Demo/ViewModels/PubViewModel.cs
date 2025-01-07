using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XPrism.Core.BindableBase;
using XPrism.Core.DI;
using XPrism.Core.Events;
using XPrism.Demo.Model;

namespace XPrism.Demo.ViewModels;

[AutoRegister(ServiceLifetime.Singleton, nameof(PubViewModel))]
public partial class PubViewModel : ViewModelBase {
    private readonly IEventAggregator _eventAggregator;
    [ObservableProperty] private string _content;

    public PubViewModel(IEventAggregator eventAggregator) {
        _eventAggregator = eventAggregator;
    }

    [RelayCommand]
    private async Task Login() {
        var r = new Random();
        var userInfo = new UserInfo {
            Username = $"john_doe{r.Next(10000, 99999)}",
            Role = "Admin1"
        };
        Content = _eventAggregator.GetEvent<UserLoggedInEvent>().Publish(userInfo, "Value")
            .GetValue<string>() ?? "NULL ERROR";
    }

    [RelayCommand]
    private async Task LoginToken() {
        var r = new Random();
        var userInfo = new UserInfo {
            Username = $"john_doe{r.Next(10000, 99999)}",
            Role = "Admin1"
        };
        Content = _eventAggregator.GetEvent<UserLoggedInEvent>().Publish(userInfo)
            .GetValue<string>() ?? "NULL ERROR";
    }

    [RelayCommand]
    private async Task Login1() {
        var r = new Random();
        var userInfo = new UserInfo {
            Username = $"john_doe{r.Next(10000, 99999)}",
            Role = "Admin"
        };


        Content = _eventAggregator.GetEvent<UserLoggedInEvent>().Publish<string>(userInfo, "Value")
            .GetValue<string>() ?? "NULL ERROR";
    }

    [RelayCommand]
    private async Task LoginToken1() {
        var r = new Random();
        var userInfo = new UserInfo {
            Username = $"john_doe{r.Next(10000, 99999)}",
            Role = "Admin"
        };


        Content = _eventAggregator.GetEvent<UserLoggedInEvent>().Publish(userInfo)
            .GetValue<string>() ?? "NULL ERROR";
    }
}