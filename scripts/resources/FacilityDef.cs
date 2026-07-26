using Godot;

[GlobalClass, Tool]
public partial class FacilityDef : Resource
{
    [Export] public string Id;                 // "crypt_garden"
    [Export] public string DisplayName;
    [Export] public int SlotCount = 2;
    [Export] public Godot.Collections.Dictionary<string, int> Inputs;   // {"ichor_root": 2}
    [Export] public Godot.Collections.Dictionary<string, int> Outputs;  // {"stew": 1}
    [Export] public float CycleSeconds = 8f;
    [Export] public float CatchRadius = 2f;
    [Export] public int BufferSize = 10;
}