namespace XPrism.Core.DI;

internal class ServiceProvider : IServiceProvider
{
    private readonly MicrosoftDependencyInjectionContainerExtension _container;

    public ServiceProvider(MicrosoftDependencyInjectionContainerExtension container)
    {
        _container = container;
    }

    public object GetService(Type serviceType)
    {
        try
        {
            return _container.Resolve(serviceType);
        }
        catch
        {
            return null;
        }
    }
}