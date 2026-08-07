using Godot;


[GlobalClass, Tool]
public partial class RitualCircleResource : Resource
{
    [Export] public float Radius = 100f;
    [Export(PropertyHint.Range, "0,360,10,radians_as_degrees")] public float AngleOffset = 0;
    [Export] public int NumElements = 3;
    [Export] public float ElementRadius = 20f;
}