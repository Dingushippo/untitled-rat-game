
using Godot;
using Godot.Collections;
using System.Collections.Generic;
using GDictionary = Godot.Collections.Dictionary<string, int>;

public class Inventory : IInventory
{
    private GDictionary _items = new();
    private readonly HashSet<string> _filter; // null means anything goes
    public int Capacity {get;}
    public int Total {get; private set;}
    public IReadOnlyDictionary<string, int> Contents => _items;
    
    public Inventory(int capacity, IEnumerable<string> filter = null)
    {
        Capacity = capacity;
        _filter = filter is null ? null : new HashSet<string>(filter);
    }

    public bool Accepts(string item) => _filter is null || _filter.Contains(item);
    public int CountOf(string item) => _items.GetValueOrDefault(item);
    public int SpaceFor(string item) => Accepts(item) ? Capacity - Total : 0;
    public int Add(string item, int amount)
    {
        int moved = Mathf.Min(amount, SpaceFor(item));
        if (moved <= 0) return 0;
        _items[item] += moved;
        Total += moved;
        return moved;
    }
    public int Add(GDictionary items)
    {
        int count = 0;
        foreach (var kv in items)
        {
            count += kv.Value;
            Add(kv.Key, kv.Value);
        }
        return count;
    }

    public bool CanRemove(string item, int amount) => CountOf(item) >= amount;
    
    public bool TryRemove(GDictionary items)
    {
        if (items == null) return true;
        
        foreach (var (item, amount) in items)
        {
            if (CountOf(item) < amount) return false;
        }
        foreach (var (item, amount) in items)
        {
            Remove(item, amount);
        }
        return true;
    }
    
    public int Remove(string item, int amount)
    {
        int removed = Mathf.Min(amount, CountOf(item));
        if (removed <= 0) return 0;
        if ((_items[item] -= removed) == 0) _items.Remove(item);
        Total -= removed;
        return removed;
    }
}

public static class InventoryTransfer
{
    public static GDictionary Move(IInventory from, IInventory to, int max = int.MaxValue)
    {
        GDictionary moved = new();
        foreach (string item in from.Contents.Keys)
        {
            if (max <= 0) break;
            int taken = to.Add(item, Mathf.Min(to.CountOf(item), max));
            if (taken == 0) continue;
            from.Remove(item, taken);
            moved[item] = taken;
            max -= taken;
        }
        if (moved.Count > 0) EventBus.Publish(Event.ItemsTransferred, from, to, moved);
        return moved;
    }
}