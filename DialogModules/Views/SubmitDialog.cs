using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ModelModules.Models.Dialog;
using XPrism.Core.Dialogs;

namespace DialogModules.Views;

public partial class SubmitDialog : DialogBase<SubmitModel?>, ISubmitable {
    [ObservableProperty] private SubmitModel? _model = new SubmitModel();

    public SubmitDialog() {
        SubmitCommand = new RelayCommand(Submit);
        CancelCommand = new RelayCommand(Cancel);
    }


    public ICommand? SubmitCommand { get; }
    public ICommand? CancelCommand { get; }

    public void Submit() {
        Result = this.Model;
        Close();
    }

    public void Cancel() {
        Result = null;
        Close();
    }
}