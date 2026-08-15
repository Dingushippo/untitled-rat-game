using Godot;

public partial class NavigationRegion3d : NavigationRegion3D
{
    public override void _EnterTree()
    {
        EventBus.Subscribe<ObjectPlaced>(OnObjectPlaced);
    }

    public override void _ExitTree()
    {
        EventBus.Unsubscribe<ObjectPlaced>(OnObjectPlaced);
    }

    public override void _Ready()
    {
        EventBus.Publish(new NavigationRegionReady(this));
    }

    private void OnObjectPlaced(ObjectPlaced _)
    {
        BakeNavigationMesh(true);
    }
}