using System;
using System.Collections.Generic;

public partial class EventBus
{
    public static void Subscribe<T>(Action<T> cb)
    {
        if (Channel<T>.Subscribers == null)
        {
            Channel<T>.Subscribers = new();
        }
        Channel<T>.Subscribers.Add(cb);
    }
    public static void Unsubscribe<T>(Action<T> cb)
    {
        Channel<T>.Subscribers.Remove(cb);
    }
    public static void Publish<T>(in T evt)
    {
        if (Channel<T>.Subscribers == null)
            return;

        foreach (Action<T> cb in Channel<T>.Subscribers)
        {
            cb?.Invoke(evt);
        }
    }
}

public static class Channel<T>
{
    internal static List<Action<T>> Subscribers;
}