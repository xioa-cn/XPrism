using System.Windows;

namespace XPrism.Core.Co;

public static class CloseApplication {
    public static void ShutdownApplication() {
        DllManager.UnloadAll();
        Application.Current.Shutdown();
        Environment.Exit(0);
    }
}