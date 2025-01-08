namespace XPrism.Core.Dialogs;

/// <summary>
/// 对话框包装器，用于将泛型对话框转换为统一的对象类型
/// </summary>
/// <typeparam name="T">原始对话框结果类型</typeparam>
internal class DialogWrapper<T> : IDialog<object>
{
    private readonly IDialog<T> _innerDialog;

    /// <summary>
    /// 初始化对话框包装器
    /// </summary>
    /// <param name="dialog">要包装的对话框实例</param>
    public DialogWrapper(IDialog<T> dialog)
    {
        _innerDialog = dialog;
    }

    /// <summary>
    /// 获取对话框标题
    /// </summary>
    public string Title => _innerDialog.Title;

    /// <summary>
    /// 获取对话框结果
    /// </summary>
    public object Result => _innerDialog.Result;

    /// <summary>
    /// 获取对话框是否可以关闭
    /// </summary>
    public bool CanClose => _innerDialog.CanClose;

    /// <summary>
    /// 关闭对话框
    /// </summary>
    public void Close() => _innerDialog.Close();
} 