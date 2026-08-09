using Godot;


[GlobalClass, Tool]
public partial class RitualElement : Resource
{
    [Export] public string Id = "element_id";
    [Export] public string DisplayName = "element_name";
    [Export] public string Description = "element_description";
    [Export] public Vector2 Position = Vector2.Zero;
    [Export] public float Rotation = 0;
    [Export] public Texture2D Symbol; // 64x64 texture
}