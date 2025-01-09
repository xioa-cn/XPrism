using CommunityToolkit.Mvvm.Input;
using XPrism.Core.BindableBase;
using XPrism.Core.DI;
using XPrism.Core.Events;
using XPrism.Demo.Model;

namespace XPrism.Demo.ViewModels;

[AutoRegister(ServiceLifetime.Singleton, nameof(SubViewModel))]
public partial class SubViewModel : ViewModelBase {
    private SubscriptionToken? _token1;
    private SubscriptionToken? _token2;
    private SubscriptionToken? _token3;
    private SubscriptionToken? _token4;
    private string _username;

    public string Username {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    public SubViewModel(IEventAggregator eventAggregator):base(eventAggregator) {
       

        // 订阅事件
        _token1 = _eventAggregator.GetEvent<UserLoggedInEvent>()
            .Subscribe<string, string>(OnUserLoggedIn, ThreadOption.UIThread, true, "Value");

        _token2 = _eventAggregator.GetEvent<UserLoggedInEvent>()
            .Subscribe<string, string>(OnUserLoggedIn1, ThreadOption.UIThread, true, "Value",
                filter => filter.Role == "Admin");

        _token3 = _eventAggregator.GetEvent<UserLoggedInEvent>()
            .Subscribe(OnUserLoggedInTask, ThreadOption.UIThread, true);

        _token4 = _eventAggregator.GetEvent<UserLoggedInEvent>()
            .Subscribe(OnUserLoggedInTask1, ThreadOption.UIThread, true,
                filter => filter.Role == "Admin");
    }

    private async Task<string> OnUserLoggedInTask(UserInfo arg) {
        Username = arg.Username;
        return "无 Token:" + arg.Username;
    }

    private async Task<string> OnUserLoggedInTask1(UserInfo arg) {
        Username = arg.Username;
        return "filter 无 Token:" + arg.Username;
    }

    private async Task<string> OnUserLoggedIn1(UserInfo arg) {
        Username = arg.Username;
        return "filter 有 Token:" + arg.Username;
    }

    private async Task<string> OnUserLoggedIn(UserInfo arg) {
        Username = arg.Username;
        return "有 Token:" + arg.Username;
    }


    [RelayCommand]
    private void Unsubscribe1() {
        _eventAggregator.GetEvent<UserLoggedInEvent>().Unsubscribe(_token1);
    }

    [RelayCommand]
    private void Unsubscribe2() {
        _eventAggregator.GetEvent<UserLoggedInEvent>().Unsubscribe(_token2);
    }

    [RelayCommand]
    private void Unsubscribe3() {
        _eventAggregator.GetEvent<UserLoggedInEvent>().Unsubscribe(_token3);
    }

    [RelayCommand]
    private void Unsubscribe4() {
        _eventAggregator.GetEvent<UserLoggedInEvent>().Unsubscribe(_token4);
    }


    public void Dispose() {
        _token1?.Dispose();
    }
}