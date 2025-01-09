using System.ComponentModel;
using System.Windows;
using XPrism.Core.DI;

namespace XPrism.Core.DataContextWindow {
    /// <summary>
    /// 支持自动设置DataContext的Window基类
    /// </summary>
    public partial class XPrismWindow : Window, IDisposable {
        /// <summary>
        /// 窗口是否被真实清楚
        /// </summary>
        public bool IsReallyClosing { get; private set; }

        public XPrismWindow() {
            Loaded += OnWindowLoaded;
            this.Closing += Window_Closing;
        }

        private void Window_Closing(object? sender, CancelEventArgs e) {
            e.Cancel = true;
            this.Hide();
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e) {
            Initialize();
        }

        internal void Initialize() {
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

        public void CloseForReal() {
            this.Closing -= Window_Closing;
            this.DataContext = null;

            IsReallyClosing = true;

            this.Close();
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing) {
            if (_disposed) return;
            if (disposing)
            {
                CloseForReal();
            }

            _disposed = true;
        }

        ~XPrismWindow() {
            Dispose(false);
        }
    }
}