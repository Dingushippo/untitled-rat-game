using Godot;

[GlobalClass]
public partial class ThrowTuning : Resource
{
    [ExportGroup("Force")]
    [Export(PropertyHint.Range, "1,100,0.1")] public float ThrowForce = 7f;
    [Export(PropertyHint.Range, "1,100,0.1")] public float MaxThrowForce = 12f;

    [ExportGroup("Charge")]
    [Export(PropertyHint.Range, "0.05,5,0.05")] public float ChargeDuration = 1.5f;
    [Export(PropertyHint.Range, "0,1,0.01")] public float ChargeStartDelay = 0.2f;

    [ExportGroup("Aim")]
    [Export(PropertyHint.Range, "-45,45,0.5")] public float AngleAdjust = 0f;
    [Export(PropertyHint.Range, "0,1,0.1")] public float AimGain = 1f;
    [Export(PropertyHint.Range, "0,50,1")] public float MaxAimCorrectionDegrees = 10f;
    [Export(PropertyHint.Range, "0,50,1")] public float MinAimDistance = 2f;

    [ExportGroup("Path Simulation")]
    [Export(PropertyHint.Range, "0.005,0.1,0.005")] public float Step = 0.02f;
    [Export(PropertyHint.Range, "50,2000,1")] public int MaxPoints = 250;
    [Export] public int SkipFrames = 2;

    [ExportGroup("Arc")]
    [Export(PropertyHint.Range, "0.1,10,0.1")] public float AscentGravityScale = 1.5f;
    [Export(PropertyHint.Range, "0.1,20,0.1")] public float DescentGravityScale = 4f;

    /// <summary>Downward speed at which gravity reaches its full descent scale.</summary>
    [Export(PropertyHint.Range, "0.1,20,0.1")] public float DescentBlendSpeed = 4f;
}
