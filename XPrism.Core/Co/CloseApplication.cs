using System.Windows;

namespace XPrism.Core.Co;

public static class CloseApplication {
    public static void ShutdownApplication() {
        ConfigDllManager.  DllManager.UnloadAll();
        Application.Current.Shutdown();
        Environment.Exit(0);
    }
}