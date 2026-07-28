using Godot;
using System.Collections.Generic;

public interface IInventory
{
    int Capacity {get;}
    int Total {get;}
    IReadOnlyDictionary<string, int> Contents {get;}
    bool Accepts(string item);
    int CountOf(string item);
    int SpaceFor(string item);
    int Add(string item, int amount);
    int Remove(string item, int amount);
}