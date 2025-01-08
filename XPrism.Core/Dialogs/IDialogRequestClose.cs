namespace XPrism.Core.Dialogs;

public interface IDialogRequestClose {
    /// <summary>
    /// 请求关闭对话框事件
    /// </summary>
    public event EventHandler? RequestClose;
}