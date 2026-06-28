using System;
using System.Collections.Generic;

public partial class EventBus
{
	private static readonly Dictionary<Type, List<Delegate>> Subscribers = new();

	public static void Subscribe<T>(Action<T> callback)
	{
		Type type = typeof(T);
		if (!Subscribers.TryGetValue(type, out List<Delegate> value))
		{
			value = new List<Delegate>();
			Subscribers[type] = value;
		}

		value.Add(callback);
	}

	public static void Unsubscribe<T>(Action<T> callback)
	{
		Type type = typeof(T);
		if (Subscribers.TryGetValue(type, out List<Delegate> list))
		{
			list.Remove(callback);
		}
	}

	public static void Publish<T>(T evt)
	{
		Type type = typeof(T);
		if (!Subscribers.TryGetValue(type, out List<Delegate> list)) return;
		foreach (Delegate cb in list.ToArray())
		{
			((Action<T>)cb)?.Invoke(evt);
		}
	}
}
