using Godot;
using Godot.Collections;

[GlobalClass, Tool]
public partial class ProductionDef : FacilityDef
{
    [Export] public int SlotCount = 2;
    [Export] public Dictionary<string, int> Inputs;   // {"ichor_root": 2}
    [Export] public Dictionary<string, int> Outputs;  // {"stew": 1}
    [Export] public float CycleSeconds = 8f;
    [Export] public int BufferSize = 10;
    [Export] public float BufferPenalty = 3f;

    /// <summary>Fraction of the output buffer that may fill before production is throttled.</summary>
    [Export(PropertyHint.Range, "0, 1.0")] public float BufferPenaltyRatio = 0.5f;

    public bool HasInputs => Inputs is { Count: > 0 };
}