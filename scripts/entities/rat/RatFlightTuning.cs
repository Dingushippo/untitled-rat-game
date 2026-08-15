using Godot;

[GlobalClass]
public partial class RatFlightTuning : Resource
{
    [ExportGroup("Speed")]
    /// <summary>Scales the simulated ballistic speed during playback. 1 = true to the simulation.</summary>
    [Export(PropertyHint.Range, "0.1,3,0.05")] public float SpeedScale = 1f;
    [Export] public float DescentGravityScale = 4f;

    /// <summary>Floor so a near-vertical throw doesn't hang motionless at its apex.</summary>
    [Export(PropertyHint.Range, "0.1,10,0.1")] public float MinSpeed = 2f;
    [Export] public float GroundProbeDistance { get; internal set; }

    [ExportGroup("Rotation")]
    [Export(PropertyHint.Range, "1,30,0.5")] public float TurnSpeed = 10f;
    [Export(PropertyHint.Range, "0.05,5,0.05")] public float LookAheadDistance = 0.75f;
    [Export(PropertyHint.Range, "0,89,1")] public float MaxPitchDegrees = 35f;

    [ExportGroup("Slot Approach")]
    [Export(PropertyHint.Range, "0,5,0.1")] public float ApproachBlendDistance = 1.5f;
    [Export(PropertyHint.Range, "0.05,2,0.05")] public float SettleDuration = 0.35f;
}