using System.Windows.Input;

namespace XPrism.Core.Dialogs;

public interface ICloseable {
    public ICommand CloseCommand { get; }
}