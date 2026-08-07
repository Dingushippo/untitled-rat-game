using Godot;

public abstract partial class RitualElement : Resource
{
    [Export] public bool Visible;
    [Export] public Vector2 Position;
    [Export] public Vector2 Rotation;

    public abstract void Draw (CanvasItem canvas);
}