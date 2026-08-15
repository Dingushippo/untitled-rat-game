using Extensions;
using Godot;

[GlobalClass, Tool]
public partial class PlaceableObject : Node3D, IPooledObject
{
    private ObjectResource _objectResource;

    [Export]
    public ObjectResource objectResource
    {
        get => _objectResource;
        set => SetObjectResource(value);
    }
    public bool IsActive { get; set; }

    [Export]
    public MeshInstance3D MeshInstance;

    public string ObjectName;
    public string Description;

    public Vector3[] SnapPoints;

    private CollisionShape3D _collider;

    public override void _PhysicsProcess(double delta)
    {
        if (!IsActive)
            return;
    }

    private void SetObjectResource(ObjectResource newResource)
    {
        _objectResource = newResource;

        if (newResource == null || MeshInstance == null)
            return;

        MeshInstance.ClearChildren();

        // TODO add support for dynamic mesh checking
        MeshInstance.Mesh = newResource.Meshes[MeshPosition.Main];
        MeshInstance.CreateConvexCollision();

        _collider = (CollisionShape3D)MeshInstance.FindChild("CollisionShape3D");
    }

    public void OnSpawn()
    {
        IsActive = true;
        Show();
        SetPhysicsProcess(true);

        if (_collider != null)
            _collider.Disabled = false;
    }

    public void OnDespawn()
    {
        IsActive = false;
        Hide();
        SetPhysicsProcess(false);

        if (_collider != null)
            _collider.Disabled = true;
    }
}