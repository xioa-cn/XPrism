using XPrism.Core.DI;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace XPrism.Core.Dialogs;

/// <summary>
/// 对话框展示实现，负责管理对话框的显示和生命周期
/// </summary>
public class DialogPresenter : IDialogPresenter {
    private readonly Stack<IDialog<object>?> _dialogStack = new();
    private readonly IContainerProvider _container;

    /// <summary>
    /// 获取当前显示的对话框
    /// </summary>
    public IDialog<object>? CurrentDialog => _dialogStack.Count > 0 ? _dialogStack.Peek() : null;

    /// <summary>
    /// 对话框打开时触发的事件
    /// </summary>
    public event EventHandler<IDialog<object>>? DialogOpened;

    /// <summary>
    /// 对话框关闭时触发的事件
    /// </summary>
    public event EventHandler<IDialog<object>>? DialogClosed;

    /// <summary>
    /// 初始化对话框展示器
    /// </summary>
    /// <param name="container">容器提供者</param>
    public DialogPresenter(IContainerProvider container) {
        _container = container;
    }

    /// <summary>
    /// 显示对话框
    /// </summary>
    /// <typeparam name="TResult">对话框结果类型</typeparam>
    /// <param name="dialog">要显示的对话框</param>
    /// <returns>对话框的处理结果</returns>
    public async Task<TResult> ShowDialogAsync<TResult>(IDialog<TResult> dialog) {
        var dialogWrapper = new DialogWrapper<TResult>(dialog);
        _dialogStack.Push(dialogWrapper);

        try
        {
            OnDialogOpened(dialogWrapper);
            var view = CreateDialogView(dialog);

            // 创建TaskCompletionSource来处理对话框结果
            var dialogResult = ((DialogBase<TResult>)dialog).ShowAsync();

            var window = ShowDialogView(view, dialog);

            // 当窗口关闭时，确保对话框也关闭
            window.Closed += (s, e) =>
            {
                if (dialog is DialogBase<TResult> baseDialog && baseDialog.CanClose)
                {
                    baseDialog.Close();
                }
            };

            // 当对话框请求关闭时，关闭窗口
            if (dialog is DialogBase<TResult> baseDialog)
            {
                baseDialog.RequestClose += (s, e) => { window.Close(); };
            }

            // 显示对话框并等待结果
            window.ShowDialog();

            // 等待并返回结果
            return await dialogResult;
        }
        finally
        {
            CloseDialog();
        }
    }

    /// <summary>
    /// 关闭当前对话框
    /// </summary>
    public void CloseDialog() {
        if (_dialogStack.Count > 0)
        {
            var dialog = _dialogStack.Pop();
            OnDialogClosed(dialog);
        }
    }

    /// <summary>
    /// 关闭所有对话框
    /// </summary>
    public void CloseAllDialogs() {
        while (_dialogStack.Count > 0)
        {
            CloseDialog();
        }
    }

    /// <summary>
    /// 触发对话框打开事件
    /// </summary>
    /// <param name="dialog">打开的对话框</param>
    protected virtual void OnDialogOpened(IDialog<object> dialog) {
        DialogOpened?.Invoke(this, dialog);
    }

    /// <summary>
    /// 触发对话框关闭事件
    /// </summary>
    /// <param name="dialog">关闭的对话框</param>
    protected virtual void OnDialogClosed(IDialog<object>? dialog) {
        DialogClosed?.Invoke(this, dialog);
    }

    /// <summary>
    /// 创建对话框视图
    /// </summary>
    /// <param name="dialog">对话框实例</param>
    /// <returns>创建的视图实例</returns>
    private object? CreateDialogView(object dialog) {
        var dialogType = dialog.GetType();
        var viewType = ResolveViewType(dialogType);

        if (viewType == null)
            throw new Exception($"Could not resolve view type: {dialogType}");

        return _container.Resolve(viewType);
    }

    /// <summary>
    /// 显示对话框视图
    /// </summary>
    /// <param name="view">要显示的视图</param>
    /// <param name="dialogViewModel"></param>
    private Window ShowDialogView(object? view, object dialogViewModel) {
        if (view is not FrameworkElement element)
            throw new InvalidOperationException("View must be a FrameworkElement");
        var owner = Application.Current.MainWindow;
        // 获取当前显示的 窗口
        foreach (Window itemWindow in Application.Current.Windows)
        {
            if (!itemWindow.IsActive) continue;
            owner = itemWindow;
            break;
        }

        var window = new Window {
            Content = element,
            Owner = owner,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            SizeToContent = SizeToContent.WidthAndHeight,
            ResizeMode = ResizeMode.NoResize,
            WindowStyle = WindowStyle.None,
            Background = Brushes.Transparent,
            AllowsTransparency = true
        };

        // 设置数据上下文
        element.DataContext = dialogViewModel;

        return window;

    }

    /// <summary>
    /// 解析对话框对应的视图类型
    /// </summary>
    /// <param name="dialogType">对话框类型</param>
    /// <returns>对应的视图类型</returns>
    private Type? ResolveViewType(Type dialogType) {
        return DialogPresenterHelper.GetDialogType(dialogType);
    }
}