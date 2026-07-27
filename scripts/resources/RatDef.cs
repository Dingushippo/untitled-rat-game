using Godot;

[GlobalClass, Tool]
public partial class RatDef : Resource
{
    [Export] public string Id;
    [Export] public string[] Traits;      // see [[Traits]]
    [Export] public float WorkRate = 1f;
}