using Godot;
using Godot.Collections;

[GlobalClass, Tool]
public partial class FacilityDef : Resource
{
    [Export] public string Id;                 // "crypt_garden"
    [Export] public string DisplayName;
    [Export] public int SlotCount = 2;
    [Export] public Dictionary<string, int> Inputs;   // {"ichor_root": 2}
    [Export] public Dictionary<string, int> Outputs;  // {"stew": 1}
    [Export] public float CycleSeconds = 8f;
    [Export] public float CatchRadius = 2f;
    [Export] public int BufferSize = 10;

    /// <summary>Fraction of the output buffer that may fill before production is throttled.</summary>
    [Export(PropertyHint.Range, "0, 1.0")] public float BufferPenaltyRatio = 0.5f;

    public bool HasInputs => Inputs is { Count: > 0 };
}