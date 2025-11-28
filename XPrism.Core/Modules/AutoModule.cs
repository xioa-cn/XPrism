using System.Reflection;
using XPrism.Core.Attribute;
using XPrism.Core.DI;
using XPrism.Core.Navigations;

namespace XPrism.Core.Modules
{
    public abstract class AutoModule<T> : IModule
    {
        public virtual void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var regionManager = containerRegistry.Resolve<IRegionManager>();
            var types = typeof(T).Assembly.GetTypes();

            var findAutoRegisterType = types.Where(e =>
                e.GetCustomAttribute<AutoRegisterViewAttribute>() != null);

            foreach (var type in findAutoRegisterType)
            {
                var attr = type.GetCustomAttribute<AutoRegisterViewAttribute>();

                if (attr == null)
                {
                    continue;
                }

                if (attr.ViewModelType != null)
                {
                    regionManager.RegisterForNavigation(type, attr.ViewModelType, attr.Region, attr.ViewName);
                }
                else
                {
                    regionManager.RegisterViewWithRegion(attr.Region, type, attr.ViewName);
                }
            }
        }

        public virtual void OnInitialized(IContainerProvider containerProvider)
        {
        }
    }
}