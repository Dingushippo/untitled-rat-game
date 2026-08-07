using Godot;


[GlobalClass, Tool]
public partial class RitualCircleResource : Resource
{
    [Export] public float Radius;
    [Export(PropertyHint.Range, "0,360,10,radians_as_degrees")] public float AngleOffset = 0;
    [Export] public int NumElements;
    [Export] public float ElementRadius;
}