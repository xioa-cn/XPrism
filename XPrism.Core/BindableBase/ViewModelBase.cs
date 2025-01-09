using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using XPrism.Core.Events;

namespace XPrism.Core.BindableBase;

public partial class ViewModelBase : ObservableObject {
    public readonly IEventAggregator _eventAggregator;

    public ViewModelBase() {
    }

    public ViewModelBase(IEventAggregator? eventAggregator) {
        _eventAggregator = eventAggregator;
    }

    protected virtual bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null) {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    protected virtual bool SetProperty<T>(ref T field, T value, Action onChanged,
        [CallerMemberName] string? propertyName = null) {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        onChanged?.Invoke();
        OnPropertyChanged(propertyName);
        return true;
    }
}