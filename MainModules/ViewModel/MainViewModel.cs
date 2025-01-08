using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XPrism.Core.BindableBase;
using XPrism.Core.Dialogs;

namespace MainModules.ViewModel;

public partial class MainViewModel : ViewModelBase {
    [ObservableProperty] private string _title;
    private readonly IDialogService _dialogService;

    public MainViewModel(IDialogService dialogService) {
        _dialogService = dialogService;
        Title = "Main Module";
    }

    /// <summary>
    /// 显示消息对话框
    /// </summary>
    [RelayCommand]
    public async Task ShowMessageExample() {
        await _dialogService.ShowMessageAsync("这是一条消息", "提示");
    }
    
    /// <summary>
    /// 显示确认对话框
    /// </summary>
    [RelayCommand]
    public async Task DeleteItemExample() {
        if (await _dialogService.ShowConfirmAsync("确定要删除这条记录吗？", "确认删除"))
        {
           
        }
    }


    /// <summary>
    /// 显示错误对话框
    /// </summary>
    [RelayCommand]
    public async Task ShowErrorExample() {
        await _dialogService.ShowErrorAsync("操作失败，请重试", "错误");
    }

    /// <summary>
    /// 显示自定义对话框
    /// </summary>
    [RelayCommand]
    public async Task ShowCustomDialogExample() {
        // var dialog = new CustomDialog {
        //     Title = "自定义对话框",
        // };
        //
        // var result = await _dialogService.ShowDialogAsync(dialog);
        // if (result)
        // {
        //     // 处理对话框结果
        // }
    }
}