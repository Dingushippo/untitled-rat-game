
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
// using GDictionary = Godot.Collections.Dictionary<string, int>;

public class Inventory : IInventory
{
    private Godot.Collections.Dictionary<string, int> _items = new();
    private readonly HashSet<string> _filter; // null means anything goes
    public int Capacity { get; }
    public IReadOnlyDictionary<string, int> Contents => _items;

    // public int Total { get; private set; }
    public int Total => _items.Values.Sum();
    public bool IsEmpty => Total == 0;
    public bool IsFull => Total >= Capacity;
    public Action Changed;

    public Inventory(int capacity, IEnumerable<string> filter = null)
    {
        Capacity = capacity;
        _filter = filter is null ? null : [.. filter];
    }

    public bool Accepts(string item) => _filter is null || _filter.Contains(item);
    public int CountOf(string item) => _items.TryGetValue(item, out int count) ? count : 0;
    public int SpaceFor(string item) => Accepts(item) ? Capacity - Total : 0;

    private int SetData(string item, int amount)
    {
        _items[item] = amount;
        Changed?.Invoke();
        return _items[item];
    }

    public int Add(string item, int amount)
    {
        int moved = Mathf.Min(amount, SpaceFor(item));
        if (moved <= 0) return 0;
        // Godot dictionaries throw on a missing key, so seed the entry through CountOf.
        SetData(item, CountOf(item) + moved);

        return moved;
    }

    /// <summary>True when every entry fits, filter and capacity included.</summary>
    public bool CanAdd(Godot.Collections.Dictionary<string, int> items)
    {
        if (items is null) return true;

        int required = 0;
        foreach (var (item, amount) in items)
        {
            if (!Accepts(item)) return false;
            required += amount;
        }
        return Total + required <= Capacity;
    }

    /// <summary>All-or-nothing add, so a recipe never leaves a half-written batch behind.</summary>
    public bool TryAdd(Godot.Collections.Dictionary<string, int> items)
    {
        if (!CanAdd(items)) return false;
        if (items is null) return true;

        foreach (var (item, amount) in items)
        {
            Add(item, amount);
        }
        return true;
    }

    public bool CanRemove(string item, int amount) => CountOf(item) >= amount;

    public bool Has(Godot.Collections.Dictionary<string, int> items)
    {
        if (items is null) return true;

        foreach (var (item, amount) in items)
        {
            if (CountOf(item) < amount) return false;
        }
        return true;
    }

    /// <summary>All-or-nothing removal, so a cycle never eats half a recipe.</summary>
    public bool TryRemove(Godot.Collections.Dictionary<string, int> items)
    {
        if (!Has(items)) return false;
        if (items is null) return true;

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
        if (SetData(item, CountOf(item) - removed) == 0) _items.Remove(item);
        return removed;
    }

    public Godot.Collections.Dictionary<string, int> RemoveAll()
    {
        Godot.Collections.Dictionary<string, int> removed = new(_items);
        _items.Clear();
        Changed?.Invoke();
        return removed;
    }

    /// <summary>True when this inventory holds anything the other one would accept.</summary>
    public bool HasAnythingFor(IInventory other)
    {
        foreach (var (item, _) in _items)
        {
            if (other.SpaceFor(item) > 0) return true;
        }
        return false;
    }

    public override string ToString() =>
        _items.Count == 0
            ? "empty"
            : string.Join(", ", _items.Select(kv => $"{kv.Key} x{kv.Value}"));
}

public static class IntventoryPrint
{
    public static string PrintContent(IInventory inventory)
    {
        string output = "";
        int tithes = 0;
        foreach (var (key, value) in inventory.Contents)
        {
            ItemDef item = ItemDatabase.Get(key);
            int titheValue = item.BaseValue * value;
            output += $"{item.DisplayName} - {value}x - tithes: {titheValue}\n";
            tithes += titheValue;
        }
        output += $"\nTotal tithe value: {tithes}";
        return output;
    }
}

public static class InventoryTransfer
{
    /// <summary>Moves whatever the destination will take, up to <paramref name="max"/> items in total.</summary>
    public static Godot.Collections.Dictionary<string, int> Move(IInventory from, IInventory to, int max = int.MaxValue)
    {
        Godot.Collections.Dictionary<string, int> moved = new();

        // Snapshot: Remove mutates the source dictionary we would otherwise be iterating.
        string[] items = from.Contents.Keys.ToArray();

        foreach (string item in items)
        {
            if (max <= 0) break;

            int available = Mathf.Min(from.CountOf(item), max);
            int taken = to.Add(item, available);
            if (taken == 0) continue;

            from.Remove(item, taken);
            moved[item] = taken;
            max -= taken;
        }
        return moved;
    }
}
