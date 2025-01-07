namespace XPrism.Core.DI;

/// <summary>
/// 定义服务作用域
/// </summary>
public interface IServiceScope : IDisposable {
    /// <summary>
    /// 获取作用域的服务提供者
    /// </summary>
    IServiceProvider ServiceProvider { get; }
}