using Godot;

[GlobalClass]
public partial class PidTuning : Resource
{
    [Export(PropertyHint.Range, "0,5,0.1")]
    public float P = 1f;

    [Export(PropertyHint.Range, "0,5,0.1")]
    public float I = 0.01f;

    [Export(PropertyHint.Range, "0,5,0.1")]
    public float D = 1f;
}
