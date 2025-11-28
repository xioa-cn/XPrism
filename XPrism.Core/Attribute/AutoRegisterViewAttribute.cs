namespace XPrism.Core.Attribute
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AutoRegisterViewAttribute : System.Attribute
    {
        public AutoRegisterViewAttribute(string region, string viewName, Type? viewModelType = null)
        {
            Region = region;
            ViewName = viewName;
            ViewModelType = viewModelType;
        }

        public Type? ViewModelType { get; set; }

        public string ViewName { get; set; }

        public string Region { get; set; }
    }
}