using CommunityToolkit.Mvvm.Input;
using DialogModules.Views;
using XPrism.Core.BindableBase;
using XPrism.Core.Dialogs;

namespace HomeModules.ViewModel;

public partial class HomeViewModel:ViewModelBase {
    private readonly IDialogService _dialogService;
    public HomeViewModel(IDialogService dialogService) {
        _dialogService = dialogService;
    }
    [RelayCommand]
    private async Task ShowDialog() {
        var dialog = new SubmitDialog() {
            Title = "自定义对话框",
        };

        var result = await _dialogService.ShowDialogAsync(dialog);
    }
}