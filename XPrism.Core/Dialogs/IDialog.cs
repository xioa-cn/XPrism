namespace XPrism.Core.Dialogs;

/// <summary>
/// 对话框接口，定义对话框的基本属性和方法
/// </summary>
/// <typeparam name="TResult">对话框返回结果的类型</typeparam>
public interface IDialog<TResult>
{
    /// <summary>
    /// 对话框标题
    /// </summary>
    string Title { get; }

    /// <summary>
    /// 对话框的处理结果
    /// </summary>
    TResult Result { get; }

    /// <summary>
    /// 指示对话框是否可以关闭
    /// </summary>
    bool CanClose { get; }

    /// <summary>
    /// 关闭对话框
    /// </summary>
    void Close();
} 