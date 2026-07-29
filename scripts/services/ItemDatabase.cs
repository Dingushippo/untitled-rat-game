using Godot;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public partial class ItemDatabase : Node
{
    private const string ITEM_RESOURCE_PATH = "res://resources/items";
    private const string FACILITY_RESOURCE_PATH = "res://resources/facilities";
    private static Dictionary<string, ItemDef> _items = new();

    public override void _EnterTree()
    {
        PopulateItems();
        ValidateFacilities();
    }

    public bool TryGetResourceFile(string file, out string fileTrimmed)
    {
        fileTrimmed = file.TrimSuffix(".remap");
        if (!fileTrimmed.EndsWith(".tres"))
        {
            GD.PushError($"{file} is not a Godot resource");
            return false;
        }
        return true;
    }

    private void PopulateItems()
    {
        _items = new();
        string[] files = DirAccess.GetFilesAt(ITEM_RESOURCE_PATH);
        foreach (string file in files)
        {
            if (!TryGetResourceFile(file, out string fileNameTrim))
            {
                continue;
            }

            ItemDef item = ResourceLoader.Load<ItemDef>(ITEM_RESOURCE_PATH.PathJoin(fileNameTrim));
            if (item is null)
            {
                GD.PushError($"{file} is not a valid ItemDef");
                continue;
            }
            if (string.IsNullOrEmpty(item.Id))
            {
                GD.PushError($"{file} is missing ID");
                continue;
            }
            _items[item.Id] = item;
        }
    }

    private void ValidateFacilities()
    {
        string[] files = DirAccess.GetFilesAt(FACILITY_RESOURCE_PATH);
        foreach (string file in files)
        {
            if (!TryGetResourceFile(file, out string fileNameTrim))
            {
                continue;
            }

            FacilityDef facility = ResourceLoader.Load<FacilityDef>(FACILITY_RESOURCE_PATH.PathJoin(fileNameTrim));
            if (facility is null)
            {
                GD.PrintErr($"{file} is not a valid FacilityDef");
                continue;
            }

            HashSet<string> keys = [.. facility.Inputs.Keys, .. facility.Outputs.Keys];
            foreach (string key in keys)
            {
                if (!_items.ContainsKey(key))
                {
                    GD.PushError($"{file} contains item missing ItemDef: {key}");
                }
            }
        }
    }

    public static ItemDef Get(string id)
    {
        if (TryGet(id, out ItemDef def))
        {
            return def;
        }
        GD.PrintErr($"Invalid item id: {id}");
        return null;
    }

    public static bool TryGet(string id, out ItemDef def)
    {
        return _items.TryGetValue(id, out def);
    }
}