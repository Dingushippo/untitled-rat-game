using Godot;

[GlobalClass, Tool]
public partial class PolygonElement : RitualElement
{
    [Export] public int Sides;
    [Export] public float Radius;
    [Export] public bool Inscribed;
}