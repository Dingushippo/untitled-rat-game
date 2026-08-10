using Godot;

[GlobalClass]
public partial class PlayerCameraTuning : Resource
{
    [Export] public float Sensitivity = 0.15f; // degrees per pixel
    [Export] public float MinPitch = -89f;
    [Export] public float MaxPitch = 89f;

    [ExportGroup("Throw Charge")]
    [Export] public float ChargePitchDegrees = 1.2f;
    [Export] public float ChargePullDistance = 0.05f;
    [Export] public float ChargeFovZoom = 6f;
    [Export] public float ChargeReturnDuration = 0.18f;

    [ExportGroup("Throw Impact")]
    [Export] public float ImpactPitchDegrees = 2.2f;
    [Export] public float ImpactRollDegrees = 1.4f;
    [Export] public float ImpactPunchDistance = 0.07f;
    [Export] public float ImpactFovPunch = 7f;
    [Export] public float MinImpactScale = 0.3f;
    [Export(PropertyHint.Range, "0,0.5")] public float ImpactAttackRatio = 0.18f;
}