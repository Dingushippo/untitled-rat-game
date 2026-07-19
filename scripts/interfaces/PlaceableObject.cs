using Godot;

[GlobalClass, Tool]
public partial class PlaceableObject : Node3D
{
    private ObjectResource _objectResource;

    [Export]
    public ObjectResource objectResource
    {
        get => _objectResource;
        set => SetObjectResource(value);
    }

    [Export] public MeshInstance3D meshInstance;

    public string name;
    public string description;

    public Vector3[] snapPoints;

    private void SetObjectResource(ObjectResource newResource)
    {
        _objectResource = newResource;

        if (newResource == null || meshInstance == null)
            return;

        // TODO add support for dynamic mesh checking
        meshInstance.Mesh = newResource.meshes[MeshPosition.Main];
        meshInstance.CreateConvexCollision();
    }
}