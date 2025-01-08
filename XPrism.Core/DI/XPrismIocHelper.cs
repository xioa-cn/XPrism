namespace XPrism.Core.DI;

public static class XPrismIoc {
    public static object? Fetch(string resourceKey) {
        return XPrism.Core.DI.ContainerLocator.Container
            .GetService(resourceKey);
    }
}