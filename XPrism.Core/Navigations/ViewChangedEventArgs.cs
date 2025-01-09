namespace XPrism.Core.Navigations;

public class ViewChangedEventArgs : EventArgs
{
    /// <summary>
    /// 旧视图
    /// </summary>
    public object? OldView { get; }

    /// <summary>
    /// 新视图
    /// </summary>
    public object? NewView { get; }

    public ViewChangedEventArgs(object? oldView, object? newView)
    {
        OldView = oldView;
        NewView = newView;
    }
}