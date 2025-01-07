using System.Diagnostics;

namespace XPrism.Core.Events;

/// <summary>
/// 事件基类，提供事件订阅和发布的基础实现
/// </summary>
public abstract class EventBase {
    /// <summary>
    /// 存储事件订阅的弱引用集合
    /// </summary>
    private readonly List<IEventSubscription> _subscriptions = new();

    /// <summary>
    /// 添加新的事件订阅
    /// </summary>
    /// <param name="subscription">事件订阅实例</param>
    protected void InternalSubscribe(IEventSubscription subscription) {
        lock (_subscriptions)
        {
            // 如果已存在相同Token的订阅，先移除旧的
            if (subscription.SubscriptionToken?.Tag != null)
            {
                var existingSubscription = _subscriptions.FirstOrDefault(s =>
                    s.SubscriptionToken?.Tag == subscription.SubscriptionToken.Tag);
                if (existingSubscription != null)
                {
                    _subscriptions.Remove(existingSubscription);
                }
            }

            // 如果是无Token的订阅，检查是否已存在相同的Action
            else if (_subscriptions.Any(s => s.Action == subscription.Action))
            {
                return; // 已存在相同的订阅，不重复添加
            }

            _subscriptions.Add(subscription);
        }
    }

    /// <summary>
    /// 通过委托移除指定的事件订阅
    /// </summary>
    /// <param name="subscriber">要移除的订阅者委托</param>
    protected void InternalUnsubscribe(Action<object> subscriber) {
        lock (_subscriptions)
        {
            var subscriptionsToRemove = _subscriptions.Where(evt => evt.Action == subscriber).ToList();
            foreach (var subscription in subscriptionsToRemove)
            {
                _subscriptions.Remove(subscription);
            }
        }
    }

    /// <summary>
    /// 通过订阅令牌移除指定的事件订阅
    /// </summary>
    /// <param name="token">订阅令牌</param>
    protected void InternalUnsubscribe(SubscriptionToken? token) {
        if (token == null) return;
        lock (_subscriptions)
        {
            var subscriptionsToRemove = _subscriptions
                .Where(evt => evt.SubscriptionToken == token ||
                              evt.SubscriptionToken?.Tag == token.Tag)
                .ToList();
            foreach (var subscription in subscriptionsToRemove)
            {
                _subscriptions.Remove(subscription);
            }
        }
    }

    /// <summary>
    /// 通过订阅对象移除指定的事件订阅
    /// </summary>
    /// <param name="subscription">要移除的订阅对象</param>
    protected void InternalUnsubscribe(IEventSubscription subscription) {
        lock (_subscriptions)
        {
            _subscriptions.Remove(subscription);
        }
    }

    /// <summary>
    /// 清理无效订阅并通知所有有效的订阅者
    /// </summary>
    /// <param name="payload">要发送给订阅者的数据</param>
    /// <param name="targetToken">目标令牌</param>
    /// <returns>事件执行结果</returns>
    protected EventResult PruneAndNotifySubscribers(object payload, object? targetToken = null) {
        List<IEventSubscription> subsToRemove = new();
        object? lastResult = null;

        lock (_subscriptions)
        {
            // 先找出所有无效的订阅
            var inactiveSubscriptions = _subscriptions.Where(s => !s.IsActive).ToList();
            subsToRemove.AddRange(inactiveSubscriptions);

            // 执行有效的订阅
            foreach (var subscription in _subscriptions.Where(e => e.IsActive))
            {
                // 检查Token匹配
                var subscriptionTag = subscription.SubscriptionToken?.Tag;

                if (targetToken != null && subscriptionTag == null)
                {
                    continue;
                }
                
                // 修改Token匹配逻辑
                bool shouldExecute;
                if (targetToken != null && subscriptionTag != null)
                {
                    // 如果发布时指定了Token，只执行匹配的订阅者
                    shouldExecute = 
                                    subscriptionTag.GetType() == targetToken.GetType() &&
                                    subscriptionTag.Equals(targetToken);
                }
                else
                {
                    // 如果发布时没有指定Token，只执行没有Token的订阅者
                    shouldExecute = subscriptionTag == null;
                }

                if (!shouldExecute)
                {
                    continue;
                }

                if (subscription.ShouldHandle(payload))
                {
                    try
                    {
                        if (subscription.HasResult)
                        {
                            var result = subscription.ActionWithResult!(payload);
                            if (result != null)
                            {
                                lastResult = result;
                            }
                        }
                        else
                        {
                            subscription.Action(payload);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"执行订阅者方法时发生错误: {ex.Message}");
                    }
                }
            }

            // 清理无效的订阅
            foreach (var subscription in subsToRemove)
            {
                _subscriptions.Remove(subscription);
            }
        }

        return new EventResult(lastResult ?? payload);
    }
}