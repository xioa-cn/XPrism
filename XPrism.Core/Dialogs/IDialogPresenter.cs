namespace XPrism.Core.Dialogs;

/// <summary>
/// 对话框展示接口，负责对话框的显示和管理
/// </summary>
public interface IDialogPresenter
{
    /// <summary>
    /// 显示对话框
    /// </summary>
    /// <typeparam name="TResult">对话框返回结果的类型</typeparam>
    /// <param name="dialog">要显示的对话框实例</param>
    /// <returns>对话框的处理结果</returns>
    Task<TResult> ShowDialogAsync<TResult>(IDialog<TResult> dialog);

    /// <summary>
    /// 关闭当前显示的对话框
    /// </summary>
    void CloseDialog();

    /// <summary>
    /// 关闭所有已打开的对话框
    /// </summary>
    void CloseAllDialogs();

    /// <summary>
    /// 获取当前显示的对话框
    /// </summary>
    IDialog<object>? CurrentDialog { get; }

    /// <summary>
    /// 对话框打开时触发的事件
    /// </summary>
    event EventHandler<IDialog<object>> DialogOpened;

    /// <summary>
    /// 对话框关闭时触发的事件
    /// </summary>
    event EventHandler<IDialog<object>> DialogClosed;
} 