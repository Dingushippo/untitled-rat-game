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
    public bool IsActive {get; set;}

    [Export] public MeshInstance3D meshInstance;

    public string name;
    public string description;

    public Vector3[] snapPoints;

    private CollisionShape3D collider;

    public override void _PhysicsProcess(double delta)
    {
        if (!IsActive) return;
    }

    private void SetObjectResource(ObjectResource newResource)
    {
        _objectResource = newResource;

        if (newResource == null || meshInstance == null)
            return;

        meshInstance.ClearChildren();

        // TODO add support for dynamic mesh checking
        meshInstance.Mesh = newResource.meshes[MeshPosition.Main];
        meshInstance.CreateConvexCollision();

        collider = (CollisionShape3D)meshInstance.FindChild("CollisionShape3D");
    }

    public void OnSpawn()
    {
        IsActive = true;
        Show();
        SetPhysicsProcess(true);

        if (collider != null) collider.Disabled = false;
    }

    public void OnDespawn()
    {
        IsActive = false;
        Hide();
        SetPhysicsProcess(false);
        
        if (collider != null) collider.Disabled = true;
    }
}