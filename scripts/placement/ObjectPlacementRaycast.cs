using Godot;

public partial class ObjectPlacementRaycast : RayCast3D
{
    [Export]
    public bool Debug = false;

    public Vector3 LookPoint;
    private MeshInstance3D _debugMarker;

    public override void _Ready()
    {
        _debugMarker = new MeshInstance3D();
        _debugMarker.Mesh = new SphereMesh() { Radius = 0.15f, Height = 0.3f };
        AddChild(_debugMarker);
        _debugMarker.Visible = Debug;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!IsColliding())
            return;

        LookPoint = GetCollisionPoint();
        _debugMarker.GlobalPosition = LookPoint;
    }
}