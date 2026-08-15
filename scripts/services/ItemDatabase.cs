using System.Collections.Generic;
using Godot;

public partial class ItemDatabase : Node
{
    private const string ITEM_RESOURCE_PATH = "res://resources/items";
    private const string FACILITY_RESOURCE_PATH = "res://resources/facilities";
    private static Dictionary<string, ItemDef> _items = new();

    public override void _Ready()
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
            if (_items.ContainsKey(item.Id))
            {
                GD.PushError($"ID: {item.Id} is duplicate");
                continue;
            }
            _items[item.Id] = item;
        }
    }

    public static ItemDef Get(string id)
    {
        if (TryGet(id, out ItemDef def))
        {
            return def;
        }
        return default;
    }

    public static bool TryGet(string id, out ItemDef def)
    {
        return _items.TryGetValue(id, out def);
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

            ProductionDef facility = ResourceLoader.Load<ProductionDef>(
                FACILITY_RESOURCE_PATH.PathJoin(fileNameTrim)
            );
            if (facility is null)
            {
                string error = $"{file} is not a valid ProductionDef";
                GD.PushError(error);
                if (OS.IsDebugBuild())
                {
                    OS.Alert(error, "ItemDatabase error");
                    GameManager.Instance.SetDataErrorFlag();
                }
                continue;
            }

            HashSet<string> keys = [.. facility.Inputs.Keys, .. facility.Outputs.Keys];
            foreach (string key in keys)
            {
                if (!_items.ContainsKey(key))
                {
                    string error = $"{file} contains item missing ItemDef: {key}";
                    GD.PushError(error);
                    if (OS.IsDebugBuild())
                    {
                        OS.Alert(error, "ItemDatabase error");
                        GameManager.Instance.SetDataErrorFlag();
                    }
                }
            }
        }
    }
}
