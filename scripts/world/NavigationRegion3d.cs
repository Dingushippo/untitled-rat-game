using Godot;

public partial class NavigationRegion3d : NavigationRegion3D
{
    public override void _EnterTree()
    {
        EventBus.Subscribe(Event.ObjectPlaced, OnObjectPlaced);
    }

    public override void _ExitTree()
    {
        EventBus.Unsubscribe(Event.ObjectPlaced, OnObjectPlaced);
    }

    public override void _Ready()
    {
        EventBus.Publish(Event.NavigationRegionReady, this);
    }

    private void OnObjectPlaced(object[] args)
    {
        BakeNavigationMesh(true);
    }
}
