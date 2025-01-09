using CommunityToolkit.Mvvm.ComponentModel;
using XPrism.Core.BindableBase;

namespace ModelModules.Models.Dialog;

public partial class SubmitModel : ViewModelBase {
    [ObservableProperty] private string _name;
    [ObservableProperty] private string _description;
}