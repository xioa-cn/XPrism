namespace XPrism.Core.Dialogs;

/// <summary>
/// 对话框服务接口，提供显示各种对话框的方法
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// 显示消息对话框
    /// </summary>
    /// <param name="message">要显示的消息内容</param>
    /// <param name="title">对话框标题，默认为"提示"</param>
    Task ShowMessageAsync(string message, string title = "提示");

    /// <summary>
    /// 显示确认对话框
    /// </summary>
    /// <param name="message">要确认的消息内容</param>
    /// <param name="title">对话框标题，默认为"确认"</param>
    /// <returns>用户选择的结果：true表示确认，false表示取消</returns>
    Task<bool> ShowConfirmAsync(string message, string title = "确认");

    /// <summary>
    /// 显示错误对话框
    /// </summary>
    /// <param name="message">错误消息内容</param>
    /// <param name="title">对话框标题，默认为"错误"</param>
    Task ShowErrorAsync(string message, string title = "错误");

    /// <summary>
    /// 显示自定义对话框
    /// </summary>
    /// <typeparam name="TResult">对话框返回结果的类型</typeparam>
    /// <param name="dialog">自定义对话框实例</param>
    /// <returns>对话框的处理结果</returns>
    Task<TResult> ShowDialogAsync<TResult>(IDialog<TResult> dialog);
} 