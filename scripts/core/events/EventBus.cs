using System;
using System.Collections.Generic;
using System.Data;
using Godot;

public partial class EventBus
{
    // private static readonly Dictionary<Type, List<Delegate>> Subscribers = new();
    private static readonly Dictionary<Event, List<Delegate>> Subscribers = new();

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


    public static void Subscribe(Event evt, Action<object[]> callback)
    {
        if (!Subscribers.TryGetValue(evt, out List<Delegate> value))
        {
            value = new List<Delegate>();
            Subscribers[evt] = value;
        }

        value.Add(callback);
    }

    public static void Unsubscribe(Event evt, Action<object[]> callback)
    {
        if (Subscribers.TryGetValue(evt, out List<Delegate> list))
        {
            list.Remove(callback);
        }
    }

    public static void Publish(Event evt, params object[] args)
    {
        if (!Subscribers.TryGetValue(evt, out List<Delegate> list)) return;
        foreach (Delegate cb in list.ToArray())
        {
            ((Action<object[]>)cb)?.Invoke(args);
        }
    }
}

public static class Channel<T>
{
    internal static List<Action<T>> Subscribers;
}
