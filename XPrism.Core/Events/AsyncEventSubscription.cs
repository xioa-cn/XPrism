using System.Diagnostics;
using System.Windows;

namespace XPrism.Core.Events;

/// <summary>
/// 异步事件订阅的实现
/// </summary>
/// <typeparam name="TPayload">事件数据类型</typeparam>
public class AsyncEventSubscription<TPayload> : IEventSubscription {
    private readonly WeakReference<Func<TPayload, Task>>? _actionWeakReference;
    private readonly WeakReference<Func<TPayload, Task<object?>>>? _funcWeakReference;
    private readonly Func<TPayload, Task>? _strongAction;
    private readonly Func<TPayload, Task<object?>>? _strongFuncAction;
    private readonly Predicate<TPayload>? _filter;

    /// <summary>
    /// 获取线程选项
    /// </summary>
    public ThreadOption ThreadOption { get; }

    /// <summary>
    /// 获取或设置订阅令牌
    /// </summary>
    public SubscriptionToken? SubscriptionToken { get; set; }

    /// <summary>
    /// 获取订阅是否仍然有效
    /// </summary>
    public bool IsActive
    {
        get
        {
            // 如果是强引用，直接返回true
            if (_strongAction != null || _strongFuncAction != null)
            {
                return true;
            }

            // 检查弱引用是否仍然有效
            if (_actionWeakReference?.TryGetTarget(out var action) == true)
            {
                return true;
            }

            if (_funcWeakReference?.TryGetTarget(out var func) == true)
            {
                return true;
            }

            return false;
        }
    }

    /// <summary>
    /// 是否是有返回值的订阅
    /// </summary>
    public bool HasResult => 
        // 检查强引用
        _strongFuncAction != null || 
        // 检查弱引用是否仍然有效
        (_funcWeakReference?.TryGetTarget(out var _) ?? false);

    /// <summary>
    /// 初始化异步事件订阅
    /// </summary>
    /// <param name="action">异步事件处理委托</param>
    /// <param name="threadOption">线程选项</param>
    /// <param name="keepSubscriberReferenceAlive">是否是强类型</param>
    /// <param name="filter">事件过滤器</param>
    public AsyncEventSubscription(Func<TPayload, Task> action,
        ThreadOption threadOption = ThreadOption.PublisherThread,
        bool keepSubscriberReferenceAlive = false,
        Predicate<TPayload>? filter = null) {
        if (action == null) throw new ArgumentNullException(nameof(action));

        if (keepSubscriberReferenceAlive)
        {
            _strongAction = action;
        }
        else
        {
            _actionWeakReference = new WeakReference<Func<TPayload, Task>>(action);
        }

        ThreadOption = threadOption;
        _filter = filter;
    }

    public AsyncEventSubscription(Func<TPayload, Task<object?>> action,
        ThreadOption threadOption = ThreadOption.PublisherThread,
        bool keepSubscriberReferenceAlive = true,
        Predicate<TPayload>? filter = null) {
        if (action == null) throw new ArgumentNullException(nameof(action));

        if (keepSubscriberReferenceAlive)
        {
            _strongFuncAction = action;
        }
        else
        {
            _funcWeakReference = new WeakReference<Func<TPayload, Task<object?>>>(action);
        }

        ThreadOption = threadOption;
        _filter = filter;
    }

    /// <summary>
    /// 获取无返回值的委托
    /// </summary>
    protected Func<TPayload, Task>? GetAction() {
        if (_strongAction != null)
        {
            return _strongAction;
        }

        return _actionWeakReference?.TryGetTarget(out var action) == true ? action : null;
    }

    /// <summary>
    /// 获取有返回值的委托
    /// </summary>
    protected Func<TPayload, Task<object?>>? GetFuncAction() {
        if (_strongFuncAction != null)
        {
            return _strongFuncAction;
        }

        return _funcWeakReference?.TryGetTarget(out var func) == true ? func : null;
    }

    public Action<object> Action => args => {
        if (!HasResult)  // 只有在无返回值时才执行Action
        {
            ExecuteAction((TPayload)args);
        }
    };

    public Func<object, object?>? ActionWithResult => HasResult ? args => {
        var func = GetFuncAction();
        if (func != null)
        {
            return func((TPayload)args).Result;
        }
        return args;
    } : null;

    private object? ExecuteAction(TPayload payload) {
        if (payload != null && !ShouldHandle(payload))
            return null;

        var action = GetAction();
        if (action == null) return null;

        action(payload);
        return payload;
    }

    /// <summary>
    /// 检查是否应该处理事件
    /// </summary>
    /// <param name="payload">事件数据</param>
    /// <returns>如果应该处理事件则返回true</returns>
    public bool ShouldHandle(object payload) {
        return _filter == null || _filter((TPayload)payload);
    }

    /// <summary>
    /// 执行订阅动作并返回结果
    /// </summary>
    public object? Execute(object payload) {
        if (!ShouldHandle(payload)) 
            return null;

        var typedPayload = (TPayload)payload;
        var func = GetFuncAction();

        try
        {
            if (func != null)
            {
                switch (ThreadOption)
                {
                    case ThreadOption.PublisherThread:
                        return _filter != null && !_filter(typedPayload) ? null : func(typedPayload).Result;

                    case ThreadOption.UIThread:
                        if (Application.Current?.Dispatcher?.CheckAccess() ?? false)
                        {
                            return _filter != null && !_filter(typedPayload) ? null : func(typedPayload).Result;
                        }

                        return Application.Current?.Dispatcher?.Invoke(() => 
                            _filter != null && !_filter(typedPayload) ? null : func(typedPayload).Result);

                    case ThreadOption.BackgroundThread:
                        return Task.Run(() => 
                            _filter != null && !_filter(typedPayload) ? null : func(typedPayload)).Result;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                var action = GetAction();
                if (action != null && (_filter == null || _filter(typedPayload)))
                {
                    switch (ThreadOption)
                    {
                        case ThreadOption.PublisherThread:
                            action(typedPayload).Wait();
                            break;

                        case ThreadOption.UIThread:
                            if (Application.Current?.Dispatcher?.CheckAccess() ?? false)
                            {
                                action(typedPayload).Wait();
                            }
                            else
                            {
                                Application.Current?.Dispatcher?.Invoke(() => 
                                    action(typedPayload).Wait());
                            }
                            break;

                        case ThreadOption.BackgroundThread:
                            Task.Run(() => action(typedPayload)).Wait();
                            break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"执行异步订阅者方法时发生错误: {ex.Message}");
        }

        return payload;
    }
}