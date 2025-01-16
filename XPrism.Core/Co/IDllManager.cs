using System.Reflection;

namespace XPrism.Core.Co;

public interface IDllManager {
    public Assembly LoadDll(string dllPath, string? name = null);
    public void UnloadDll(string dllPath);
    public void UnloadAll();
    public bool IsLoaded(string name);
}