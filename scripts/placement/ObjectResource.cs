using System.Collections.Generic;
using Godot;


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
    [Export] public string name;
    [Export] public string description;
    [Export] public bool isTilable;
    [Export] public Godot.Collections.Dictionary<MeshPosition, Mesh> meshes;
    [Export] public Vector3[] snapPositions;
}