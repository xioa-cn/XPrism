using System.Reflection;
using System.Runtime.Loader;

namespace XPrism.Core.Co;

public class CustomAssemblyLoadContext : AssemblyLoadContext
{
    public CustomAssemblyLoadContext() : base(isCollectible: true)
    {
    }

    protected override Assembly Load(AssemblyName assemblyName)
    {
        return null;
    }
}