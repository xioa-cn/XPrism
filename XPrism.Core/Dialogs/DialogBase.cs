using XPrism.Core.BindableBase;

namespace XPrism.Core.Dialogs;

/// <summary>
/// 对话框基类，实现基本的对话框功能
/// </summary>
/// <typeparam name="TResult">对话框返回结果的类型</typeparam>
public abstract class DialogBase<TResult> : ViewModelBase, IDialog<TResult>, IDialogRequestClose {
    private bool _isOpen;
    private TaskCompletionSource<TResult> _tcs;

    /// <summary>
    /// 对话框标题
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 对话框结果
    /// </summary>
    public TResult Result { get; set; }

    /// <summary>
    /// 指示对话框是否可以关闭
    /// </summary>
    public bool CanClose { get; set; } = true;


    /// <summary>
    /// 关闭对话框
    /// </summary>
    public virtual void Close() {
        if (!CanClose) return;

        _isOpen = false;
        _tcs?.TrySetResult(Result);
        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// 显示对话框并等待结果
    /// </summary>
    /// <returns>对话框的处理结果</returns>
    /// <exception cref="InvalidOperationException">当对话框已经打开时抛出</exception>
    public Task<TResult> ShowAsync() {
        if (_isOpen)
        {
            throw new InvalidOperationException("Dialog is already open");
        }

        _isOpen = true;
        _tcs = new TaskCompletionSource<TResult>();

        return _tcs.Task;
    }

    public event EventHandler? RequestClose;
}