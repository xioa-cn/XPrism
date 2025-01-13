namespace XPrism.Core.Navigations;

public static class NavigationAwareHelper {
    public static void Push(this Dictionary<string, Stack<NavigationContext>?> navigationAware, string regionName,
        NavigationContext navigationContext) {
        if (navigationAware.TryGetValue(regionName, out var stack))
        {
            stack ??= new Stack<NavigationContext>();
            stack.Push(navigationContext);
        }
        else
        {
            var value = new Stack<NavigationContext>();
            value.Push(navigationContext);
            navigationAware.Add(regionName, value);
        }
    }

    public static void Pop(this Dictionary<string, Stack<NavigationContext>?> navigationAware, string regionName) {
        if (navigationAware.TryGetValue(regionName, out var stack))
        {
            stack.Pop();
        }
    }

    public static NavigationContext? Peek(this Dictionary<string, Stack<NavigationContext>?> navigationAware,
        string regionName) {
        return navigationAware[regionName].Peek();
    }
}