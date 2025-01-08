using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace XPrism.Core.Dialogs;

/// <summary>
/// 消息对话框，用于显示简单的消息提示
/// </summary>
public class MessageDialog : DialogBase<bool>, ICloseable {
    public ICommand CloseCommand { get; }

    /// <summary>
    /// 获取消息内容
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// 获取或设置是否为错误消息
    /// </summary>
    public bool? IsError { get; set; }

    /// <summary>
    /// 初始化消息对话框
    /// </summary>
    /// <param name="message">要显示的消息内容</param>
    /// <param name="title">对话框标题，默认为"提示"</param>
    public MessageDialog(string message, string title = "提示") {
        Message = message;
        Title = title;
        CloseCommand = new RelayCommand(this.Close);
    }
}