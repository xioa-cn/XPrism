using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace XPrism.Core.Dialogs;

/// <summary>
/// 确认对话框，用于获取用户的确认操作
/// </summary>
public class ConfirmDialog : DialogBase<bool>, ICloseable {
    public ICommand CloseCommand { get; }

    public ICommand SubmitCommand { get; }
    public ICommand CancelCommand { get; }
    
    /// <summary>
    /// 获取确认消息内容
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// 初始化确认对话框
    /// </summary>
    /// <param name="message">要确认的消息内容</param>
    /// <param name="title">对话框标题，默认为"确认"</param>
    public ConfirmDialog(string message, string title = "确认") {
        Message = message;
        Title = title;
        CloseCommand = new RelayCommand(this.Close);
        SubmitCommand = new RelayCommand(Confirm);
        CancelCommand = new RelayCommand(Cancel);
    }

    /// <summary>
    /// 确认操作
    /// </summary>
    public void Confirm() {
        Result = true;
        Close();
    }

    /// <summary>
    /// 取消操作
    /// </summary>
    public void Cancel() {
        Result = false;
        Close();
    }
}