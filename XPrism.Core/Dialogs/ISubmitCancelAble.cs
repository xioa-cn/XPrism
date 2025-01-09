using System.Windows.Input;

namespace XPrism.Core.Dialogs;

/// <summary>
/// 提交能力接口
/// </summary>
public interface ISubmitable {
    
    ICommand? SubmitCommand { get; }
    ICommand? CancelCommand { get; }
    void Submit();
    void Cancel();
}