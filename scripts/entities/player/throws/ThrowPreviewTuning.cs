using Godot;

[GlobalClass]
public partial class ThrowPreviewTuning : Resource
{
    [ExportGroup("Trail")]
    [Export(PropertyHint.Range, "0.05, 2f, 0.01")] public float DotSpacing = 0.25f;
    [Export(PropertyHint.Range, "0.01, 1f, 0.01")] public float DotSize = 0.06f;
    [Export(PropertyHint.Range, "0.05, 4f, 0.05")] public float NearFarSizeRatio = 2.0f;

    [ExportGroup("Fade")]
    [Export] public int FadeStart = 6;
    [Export] public int FadeEnd = 30;
    [Export] public float MinAlpha = 0.15f;

    [ExportGroup("Bounce")]
    [Export] public float BounceFalloff = 0.6f;
    [Export] public float ImpactRingSize = 0.16f;

    [ExportGroup("Flow")]
    [Export] public float FlowSpeed = 4.0f;
    [Export] public float FlowFrequency = 1.5f;
    [Export] public float FlowStrength = 0.5f;

    [ExportGroup("Reticle")]
    [Export] public float ReticleSize = 0.5f;
    [Export] public float ReticlePulseRate = 2.0f;
    [Export] public float SurfaceOffset = 0.02f;
    [Export] public float ReticleSpinRate = 1.0f;
    [Export] public float ReticleChevronSpeed = 1.0f;

    [ExportGroup("Highlight")]
    [Export] public float HighlightSize = 0.4f;
    [Export] public float HighlightPulseRate = 1.5f;

    [ExportGroup("Colors")]
    [Export] public Color FreeThrowColor;
    [Export] public Color SlotThrowColor;
    [Export] public Color IntakeThrowColor;
}