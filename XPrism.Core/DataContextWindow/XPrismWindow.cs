using System.Windows;
using XPrism.Core.DI;

namespace XPrism.Core.DataContextWindow
{
    /// <summary>
    /// 支持自动设置DataContext的Window基类
    /// </summary>
    public partial class XPrismWindow : Window
    {
        public XPrismWindow()
        {
            Loaded += OnWindowLoaded;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext != null) return;

            var viewModelAttribute = GetType().GetCustomAttributes(typeof(XPrismViewModelAttribute), true)
                .FirstOrDefault() as XPrismViewModelAttribute;

            if (viewModelAttribute == null)
            {
                return;
            }

            if (viewModelAttribute.ViewModelType == null)
            {
                return;
            }

            try
            {
                DataContext = viewModelAttribute.ServiceName != null
                    ? ContainerLocator.GetService(viewModelAttribute.ViewModelType, viewModelAttribute.ServiceName)
                    : ContainerLocator.GetService(viewModelAttribute.ViewModelType);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Failed to create ViewModel of type {viewModelAttribute.ViewModelType.Name}. " +
                    $"Make sure it is registered in the container.", ex);
            }
        }
    }
}