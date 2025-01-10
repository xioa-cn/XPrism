namespace XPrism.Core.Navigations;

public static class NavigationAwareHelper {
    public static void Push(this Dictionary<string, Stack<NavigationContext>?> navigationAware, string regionName,
        NavigationContext navigationContext) {
        navigationAware[regionName].Push(navigationContext);
    }

    public static void Pop(this Dictionary<string, Stack<NavigationContext>?> navigationAware, string regionName) {
        navigationAware[regionName].Pop();
    }

    public static NavigationContext? Peek(this Dictionary<string, Stack<NavigationContext>?> navigationAware,
        string regionName) {
        return navigationAware[regionName].Peek();
    }
}