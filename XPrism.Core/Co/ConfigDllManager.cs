namespace XPrism.Core.Co;

public static class ConfigDllManager {
    private static IDllManager? _dllManager;

    public static IDllManager DllManager {
        get { return _dllManager ??= new DllManager(); }
        set => _dllManager = value;
    }


    public static void SetDllManager(IDllManager manager) {
        _dllManager = manager;
    }
}