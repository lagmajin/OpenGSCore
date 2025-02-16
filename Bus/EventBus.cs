using System;
using System.Collections.Generic;

public class EventBus
{
    // 部屋のIDをキーにして、任意の型のイベントを受け取るハンドラをリストとして保持
    private readonly Dictionary<string, List<Action<object>>> _subscriptions = new Dictionary<string, List<Action<object>>>();

    // IDにサブスクライブする (ここでobject型を使うことで任意の型に対応)
    public void Subscribe(string id, Action<object> handler)
    {
        if (!_subscriptions.ContainsKey(id))
        {
            _subscriptions[id] = new List<Action<object>>();
        }
        _subscriptions[id].Add(handler);
    }

    // IDにイベントを発行 (任意の型のデータを受け取る)
    public void Publish(string id, object message)
    {
        if (_subscriptions.ContainsKey(id))
        {
            foreach (var handler in _subscriptions[id])
            {
                handler(message);
            }
        }
    }
}