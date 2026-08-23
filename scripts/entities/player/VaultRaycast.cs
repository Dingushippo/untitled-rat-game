using Godot;

public partial class VaultRaycast : RayCast3D
{
    [Export]
    public Node3D Head;

    [Export]
    public Node3D RotationNode;

    public override void _Process(double delta)
    {
        Vector3 rotation = Head.GlobalRotation * Vector3.Up;
        RotationNode.GlobalRotation = rotation;
    }
}
