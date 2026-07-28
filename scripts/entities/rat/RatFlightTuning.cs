using Godot;

[GlobalClass]
public partial class RatFlightTuning : Resource
{
    [ExportGroup("Speed")]
    [Export(PropertyHint.Range, "0.5,30,0.1")] public float MinSpeed = 3f;
    [Export(PropertyHint.Range, "0.5,30,0.1")] public float MaxSpeed = 10f;

    [ExportGroup("Rotation")]
    [Export(PropertyHint.Range, "1,30,0.5")] public float TurnSpeed = 10f;
    [Export(PropertyHint.Range, "0.05,5,0.05")] public float LookAheadDistance = 0.75f;
    [Export(PropertyHint.Range, "0,89,1")] public float MaxPitchDegrees = 35f;

    [ExportGroup("Slot Approach")]
    [Export(PropertyHint.Range, "0,5,0.1")] public float ApproachBlendDistance = 1.5f;
    [Export(PropertyHint.Range, "0.05,2,0.05")] public float SettleDuration = 0.35f;
}
