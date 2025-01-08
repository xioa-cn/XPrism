using XPrism.Core.DI;

namespace XPrism.Core.Dialogs;

/// <summary>
/// 对话框服务实现，提供显示各种对话框的方法
/// </summary>
public class DialogService : IDialogService {
    private readonly IContainerProvider _container;

    /// <summary>
    /// 初始化对话框服务
    /// </summary>
    /// <param name="container">容器提供者</param>
    public DialogService(IContainerProvider container) {
        _container = container;
    }

    /// <summary>
    /// 显示消息对话框
    /// </summary>
    /// <param name="message">要显示的消息</param>
    /// <param name="title">对话框标题</param>
    public async Task ShowMessageAsync(string message, string title = "提示") {
        var dialog = new MessageDialog(message, title);
        await ShowDialogAsync(dialog);
    }

    /// <summary>
    /// 显示确认对话框
    /// </summary>
    /// <param name="message">要确认的消息</param>
    /// <param name="title">对话框标题</param>
    /// <returns>用户的确认结果</returns>
    public async Task<bool> ShowConfirmAsync(string message, string title = "确认") {
        var dialog = new ConfirmDialog(message, title);
        return await ShowDialogAsync(dialog);
    }

    /// <summary>
    /// 显示错误对话框
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <param name="title">对话框标题</param>
    public async Task ShowErrorAsync(string message, string title = "错误") {
        var dialog = new MessageDialog(message, title) { IsError = true };
        await ShowDialogAsync(dialog);
    }

    /// <summary>
    /// 显示自定义对话框
    /// </summary>
    /// <typeparam name="TResult">对话框结果类型</typeparam>
    /// <param name="dialog">要显示的对话框</param>
    /// <returns>对话框的处理结果</returns>
    public async Task<TResult> ShowDialogAsync<TResult>(IDialog<TResult> dialog) {
        var presenter = _container.Resolve<IDialogPresenter>();
        if (presenter is null) throw new InvalidOperationException();
        return await presenter.ShowDialogAsync(dialog);
    }
}