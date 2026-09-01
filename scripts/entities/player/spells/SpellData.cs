using Godot;


[GlobalClass]
public partial class SpellData : Resource
{
    [Export] public string Id;
    [Export] public string DisplayName;
    [Export] public string Description;
    [Export] public float EssenceCost;
    [Export] public float[] LevelChargeTimes = [0f];
    [Export] public PackedScene SpellScene;
}