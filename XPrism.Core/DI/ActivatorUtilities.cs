namespace XPrism.Core.DI
{
    /// <summary>
    /// 实例创建工具类
    /// </summary>
    internal static class ActivatorUtilities
    {
        public static object CreateInstance(IContainerProvider provider, Type instanceType)
        {
            var constructor = instanceType.GetConstructors()
                .OrderByDescending(c => c.GetParameters().Length)
                .First();

            var parameters = constructor.GetParameters();
            var parameterInstances = new object[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                parameterInstances[i] = provider.Resolve(parameter.ParameterType);
            }

            return constructor.Invoke(parameterInstances);
        }
    }
} 