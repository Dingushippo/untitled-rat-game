using Godot;

[GlobalClass, Tool]
public abstract partial class RitualElement : Resource
{
    [Export] public bool Visible;
    [Export] public Vector2 Position;
    [Export] public Vector2 Rotation;
}