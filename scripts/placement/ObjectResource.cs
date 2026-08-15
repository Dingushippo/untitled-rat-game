using Godot;
using Godot.Collections;

public enum MeshPosition
{
    Main,
    Front,
    Rear,
    Left,
    Right,
}

[GlobalClass, Tool]
public partial class ObjectResource : Resource
{
    [Export]
    public string Name;

    [Export]
    public string Description;

    [Export]
    public bool IsTilable;

    [Export]
    public Dictionary<MeshPosition, Mesh> Meshes;

    [Export]
    public Vector3[] SnapPositions;
}