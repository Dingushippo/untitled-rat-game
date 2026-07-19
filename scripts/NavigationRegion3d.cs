using Godot;

public partial class NavigationRegion3d : NavigationRegion3D
{
    public override void _Ready()
    {
        EventBus.Subscribe(Event.ObjectPlaced, OnObjectPlaced);
        EventBus.Publish(Event.NavigationRegionReady, this);
    }
    private void OnObjectPlaced(object[] args)
    {
        BakeNavigationMesh(true);
    }
}
