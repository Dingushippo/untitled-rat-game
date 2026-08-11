using Godot;
using System;

public partial class HazardResource : Resource
{
    [Export] public string Id;
    [Export] public string DisplayName;
    [Export] public string Description;
    [Export] public PackedScene Scene;
}
