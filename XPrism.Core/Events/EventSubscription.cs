using System.Windows;

namespace XPrism.Core.Events;


/// <summary>
/// 事件订阅的泛型实现
/// </summary>
/// <typeparam name="TPayload">事件数据类型</typeparam>
public class EventSubscription<TPayload> : IEventSubscription {
    private readonly WeakReference<Action<TPayload>>? _actionWeakReference;
    private readonly WeakReference<Func<TPayload, object?>>? _funcWeakReference;
    private readonly Action<TPayload>? _strongAction;
    private readonly Func<TPayload, object?>? _strongFuncAction;
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

    public EventSubscription(Action<TPayload> action,
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
            _actionWeakReference = new WeakReference<Action<TPayload>>(action);
        }

        ThreadOption = threadOption;
        _filter = filter;
    }

    public EventSubscription(Func<TPayload, object?> action,
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
            _funcWeakReference = new WeakReference<Func<TPayload, object?>>(action);
        }

        ThreadOption = threadOption;
        _filter = filter;
    }

    /// <summary>
    /// 获取无返回值的委托
    /// </summary>
    protected Action<TPayload>? GetAction() {
        if (_strongAction != null)
        {
            return _strongAction;
        }

        return _actionWeakReference?.TryGetTarget(out var action) == true ? action : null;
    }

    /// <summary>
    /// 获取有返回值的委托
    /// </summary>
    protected Func<TPayload, object?>? GetFuncAction() {
        if (_strongFuncAction != null)
        {
            return _strongFuncAction;
        }

        return _funcWeakReference?.TryGetTarget(out var func) == true ? func : null;
    }

    /// <summary>
    /// 将泛型委托转换为非泛型委托
    /// </summary>
    public Action<object> Action => args => {
        if (!HasResult)  // 只有在无返回值时才执行Action
        {
            ExecuteAction((TPayload)args);
        }
    };

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
        // 先检查过滤器
        if (!ShouldHandle(payload)) 
            return null;

        var typedPayload = (TPayload)payload;
        var func = GetFuncAction();

        if (func != null)
        {
            switch (ThreadOption)
            {
                case ThreadOption.PublisherThread:
                    return _filter != null && !_filter(typedPayload) ? null : func(typedPayload);

                case ThreadOption.UIThread:
                    if (Application.Current?.Dispatcher?.CheckAccess() ?? false)
                    {
                        return _filter != null && !_filter(typedPayload) ? null : func(typedPayload);
                    }

                    return Application.Current?.Dispatcher?.Invoke(() => 
                        _filter != null && !_filter(typedPayload) ? null : func(typedPayload));

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
                        action(typedPayload);
                        break;

                    case ThreadOption.UIThread:
                        if (Application.Current?.Dispatcher?.CheckAccess() ?? false)
                        {
                            action(typedPayload);
                        }
                        else
                        {
                            Application.Current?.Dispatcher?.Invoke(() => action(typedPayload));
                        }
                        break;

                    case ThreadOption.BackgroundThread:
                        Task.Run(() => action(typedPayload));
                        break;
                }
            }
        }

        return payload;
    }

    private object? ExecuteAction(TPayload payload) 
    {
        // 先检查过滤器
        if (_filter != null && !_filter(payload))
            return null;

        var action = GetAction();
        if (action == null) 
            return null;

        action(payload);
        return payload;
    }

    public Func<object, object?>? ActionWithResult => HasResult ? args => {
        var func = GetFuncAction();
        if (func != null)
        {
            return func((TPayload)args);
        }
        return args;
    } : null;
}