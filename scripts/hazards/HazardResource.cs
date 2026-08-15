using Godot;


[GlobalClass]
public partial class HazardResource : Resource
{
    [Export] public string Id;
    [Export] public string DisplayName;
    [Export] public string Description;
    [Export] public HazardSpawnType SpawnType;
    [Export] public PackedScene Scene;
}

public enum HazardSpawnType
{
    NearFacility,
    OnFloor,
    OnWall,
    InSky,
}