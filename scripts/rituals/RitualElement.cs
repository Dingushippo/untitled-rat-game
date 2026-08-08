using Godot;

public abstract partial class RitualElement : Resource
{
    [Export] public string Id = "element_id";
    [Export] public string DisplayName = "element_name";
    [Export] public string Description = "element_description";
    [Export] public float RingRadius = 20f; 
    [Export] public Vector2 Position = Vector2.Zero;
    [Export] public Vector2 Rotation = Vector2.Zero;
}