using System.ComponentModel;
using Godot;

public partial class Rat : RigidBody3D, IPooledObject
{
    [Export]
    public CollisionShape3D Collider;

    public override void _Ready()
    {
        FreezeMode = FreezeModeEnum.Static;
        Freeze = true;
        Collider.Disabled = true;
    }

    public void OnSpawn()
    {
        ContinuousCd = true;
        Show();
    }

    public void OnDespawn()
    {
        ContinuousCd = false;
        Hide();
    }
}
