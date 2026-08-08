using Godot;
using Godot.Collections;

[GlobalClass, Tool]
public partial class RitualCircleResource : Resource
{
    [Export] public float Radius = 100f;
    [Export(PropertyHint.Range, "0,360,10,radians_as_degrees")] public float AngleOffset = 0;
    [Export] public Array<RitualElement> RitualElements = [];
    [Export] public float ElementRadius = 20f;
    public int NumElements => RitualElements.Count;
}