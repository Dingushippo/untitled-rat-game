using Godot;
using System;

public partial class ObjectPlacementRaycast : RayCast3D
{
    [Export] public bool Debug = false;

    public Vector3 lookPoint;
    private MeshInstance3D debugMarker;


    public override void _Ready()
    {
        debugMarker = new MeshInstance3D();
        debugMarker.Mesh = new SphereMesh()
        {
          Radius = 0.15f,
          Height = 0.3f
        };
        AddChild(debugMarker);
        debugMarker.Visible = Debug;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!IsColliding()) return;

        lookPoint = GetCollisionPoint();
        debugMarker.GlobalPosition = lookPoint;
    }
}
