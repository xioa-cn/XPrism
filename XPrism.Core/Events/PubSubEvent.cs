namespace XPrism.Core.Events;

/// <summary>
/// 发布订阅事件的泛型实现
/// </summary>
/// <typeparam name="TPayload">事件携带的数据类型</typeparam>
public class PubSubEvent<TPayload> : EventBase
{
    /// <summary>
    /// 创建订阅并返回订阅令牌
    /// </summary>
    private SubscriptionToken CreateSubscription<TSubscription>(
        TSubscription subscription,
        SubscriptionType subscriptionType,
        object? token = null) where TSubscription : IEventSubscription
    {
        InternalSubscribe(subscription);
        var subscriptionToken = new SubscriptionToken(
            () => Unsubscribe(subscription), 
            subscriptionType,
            token);
        subscription.SubscriptionToken = subscriptionToken;
        return subscriptionToken;
    }

    /// <summary>
    /// 订阅事件（同步版本，无返回值）
    /// </summary>
    public SubscriptionToken Subscribe(Action<TPayload> action, 
        ThreadOption threadOption = ThreadOption.PublisherThread,
        bool keepSubscriberReferenceAlive = true,
        Predicate<TPayload>? filter = null)
    {
        var subscription = new EventSubscription<TPayload>(
            action, 
            threadOption, 
            keepSubscriberReferenceAlive,
            filter);
        return CreateSubscription(subscription, SubscriptionType.Sync);
    }

    /// <summary>
    /// 订阅事件（同步版本，无返回值，带标识）
    /// </summary>
    public SubscriptionToken Subscribe<TToken>(Action<TPayload> action, 
        ThreadOption threadOption = ThreadOption.PublisherThread,
        bool keepSubscriberReferenceAlive = true,
        TToken token = default,
        Predicate<TPayload>? filter = null) where TToken : class
    {
        var subscription = new EventSubscription<TPayload>(
            action, 
            threadOption, 
            keepSubscriberReferenceAlive,
            filter);
        return CreateSubscription(subscription, SubscriptionType.Sync, token);
    }

    /// <summary>
    /// 订阅事件（同步版本，有返回值）
    /// </summary>
    public SubscriptionToken Subscribe(Func<TPayload, object?> action,
        ThreadOption threadOption = ThreadOption.PublisherThread,
        bool keepSubscriberReferenceAlive = true,
        Predicate<TPayload>? filter = null)
    {
        var subscription = new EventSubscription<TPayload>(
            action, 
            threadOption, 
            keepSubscriberReferenceAlive,
            filter);
        return CreateSubscription(subscription, SubscriptionType.SyncWithResult);
    }

    /// <summary>
    /// 订阅事件（同步版本，有返回值，带标识）
    /// </summary>
    public SubscriptionToken Subscribe<TToken, TResult>(Func<TPayload, TResult> action,
        ThreadOption threadOption = ThreadOption.PublisherThread,
        bool keepSubscriberReferenceAlive = true,
        TToken token = default,
        Predicate<TPayload>? filter = null) where TToken : class
    {
        Func<TPayload, object?> wrappedAction = payload => {
            var result = action(payload);
            return result;
        };

        var subscription = new EventSubscription<TPayload>(
            wrappedAction, 
            threadOption, 
            keepSubscriberReferenceAlive,
            filter);
        return CreateSubscription(subscription, SubscriptionType.SyncWithResult, token);
    }

    /// <summary>
    /// 订阅事件（异步版本，无返回值）
    /// </summary>
    public SubscriptionToken Subscribe(Func<TPayload, Task> action,
        ThreadOption threadOption = ThreadOption.PublisherThread,
        bool keepSubscriberReferenceAlive = true,
        Predicate<TPayload>? filter = null)
    {
        if (action.Method.ReturnType.IsGenericType)
        {
            throw new ArgumentException("For Task<T> return types, please use the overload that accepts Func<TPayload, Task<object?>>");
        }

        var subscription = new AsyncEventSubscription<TPayload>(
            action, 
            threadOption, 
            keepSubscriberReferenceAlive,
            filter);
        return CreateSubscription(subscription, SubscriptionType.Async);
    }

    /// <summary>
    /// 订阅事件（异步版本，无返回值，带标识）
    /// </summary>
    public SubscriptionToken Subscribe<TToken>(Func<TPayload, Task> action,
        ThreadOption threadOption = ThreadOption.PublisherThread,
        bool keepSubscriberReferenceAlive = true,
        TToken token = default,
        Predicate<TPayload>? filter = null) where TToken : class
    {
        var subscription = new AsyncEventSubscription<TPayload>(
            action, 
            threadOption, 
            keepSubscriberReferenceAlive,
            filter);
        return CreateSubscription(subscription, SubscriptionType.Async, token);
    }

    /// <summary>
    /// 订阅事件（异步版本，有返回值）
    /// </summary>
    public SubscriptionToken Subscribe<TResult>(Func<TPayload, Task<TResult>> action,
        ThreadOption threadOption = ThreadOption.PublisherThread,
        bool keepSubscriberReferenceAlive = true,
        Predicate<TPayload>? filter = null)
    {
        Func<TPayload, Task<object?>> wrappedAction = async payload => {
            var result = await action(payload);
            return result;
        };

        var subscription = new AsyncEventSubscription<TPayload>(
            wrappedAction, 
            threadOption, 
            keepSubscriberReferenceAlive,
            filter);
        return CreateSubscription(subscription, SubscriptionType.AsyncWithResult);
    }

    /// <summary>
    /// 订阅事件（异步版本，有返回值，带标识）
    /// </summary>
    public SubscriptionToken Subscribe<TToken, TResult>(Func<TPayload, Task<TResult>> action,
        ThreadOption threadOption = ThreadOption.PublisherThread,
        bool keepSubscriberReferenceAlive = true,
        TToken token = default,
        Predicate<TPayload>? filter = null) where TToken : class
    {
        Func<TPayload, Task<object?>> wrappedAction = async payload => {
            var result = await action(payload);
            return result;
        };

        var subscription = new AsyncEventSubscription<TPayload>(
            wrappedAction, 
            threadOption, 
            keepSubscriberReferenceAlive,
            filter);
        return CreateSubscription(subscription, SubscriptionType.AsyncWithResult, token);
    }

    /// <summary>
    /// 发布事件并获取返回值
    /// </summary>
    /// <param name="payload">要发布的数据</param>
    /// <returns>事件执行结果</returns>
    public EventResult Publish(TPayload payload)
    {
        if (payload == null) return new EventResult(null);
        return PruneAndNotifySubscribers(payload);
    }

    /// <summary>
    /// 发布事件并获取指定Token的订阅者的返回值
    /// </summary>
    /// <param name="payload">要发布的数据</param>
    /// <param name="token">订阅标识</param>
    /// <returns>事件执行结果</returns>
    public EventResult Publish<TToken>(TPayload payload, TToken token) where TToken : class
    {
        if (payload == null) return new EventResult(null);
        return PruneAndNotifySubscribers(payload, token);
    }

    /// <summary>
    /// 通过委托取消订阅
    /// </summary>
    /// <param name="subscriber">要取消的订阅委托</param>
    public void Unsubscribe(Action<TPayload> subscriber)
    {
        InternalUnsubscribe(args => subscriber((TPayload)args));
    }

    /// <summary>
    /// 通过订阅令牌取消订阅
    /// </summary>
    /// <param name="token">订阅令牌</param>
    public void Unsubscribe(SubscriptionToken? token)
    {
        InternalUnsubscribe(token);
    }

    /// <summary>
    /// 通过订阅对象取消订阅
    /// </summary>
    /// <param name="subscription">订阅对象</param>
    private void Unsubscribe(IEventSubscription subscription)
    {
        InternalUnsubscribe(subscription);
    }
} 