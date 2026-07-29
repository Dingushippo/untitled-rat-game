using Godot;

[GlobalClass, Tool]
public partial class ItemDef : Resource
{
    [Export] public string Id;
    [Export] public string DisplayName = "???";
    [Export] public int BaseValue = 0;
    [Export] public Texture2D Icon;
}